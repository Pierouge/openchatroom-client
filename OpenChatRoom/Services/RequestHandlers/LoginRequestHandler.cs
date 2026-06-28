using System.Net.Http.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using SecureRemotePassword;

public class LoginRequestHandler(ApiClient apiClient, LoginInfoManager infoManager)
{
  private readonly ApiClient api = apiClient;
  private readonly LoginInfoManager info = infoManager;

  // Technically, this is two requests to the api, but they're the two steps of the Login protocol
  public async Task<RequestResult> Login(string username, string password)
  {
    SrpClient srpClient = new();
    SrpEphemeral clientEphemeral = srpClient.GenerateEphemeral();

    HttpContent contentPhase1 = JsonContent.Create(
        new UserControllerRecords.SrpStep1Request(username, clientEphemeral.Public));

    ApiResult resultPhase1 = await api.SendRequestAsync("user/srp/1", HttpMethod.Post, contentPhase1);

    RequestResult requestResultPhase1 = await RequestResult.FromApiResult(resultPhase1);
    if (!requestResultPhase1.IsSuccess)
      return requestResultPhase1;

    // Phase 3
    UserControllerRecords.SrpStep2Response? phase2Content = await resultPhase1.Response!.Content.ReadFromJsonAsync<UserControllerRecords.SrpStep2Response>();

    if (phase2Content == null)
      return RequestResult.Failure("Error: Server replied without a content", resultPhase1);

    string privateKey = srpClient.DerivePrivateKey(phase2Content.Salt, username, password);
    SrpSession clientSession = srpClient.DeriveSession(clientEphemeral.Secret,
        phase2Content.ServerPublicEphemeral, phase2Content.Salt, username, privateKey);

    HttpContent contentPhase3 = new StringContent(
        JsonSerializer.Serialize(clientSession.Proof),
        Encoding.UTF8,
        "application/json"
    );

    ApiResult resultPhase3 = await api.SendRequestAsync("user/srp/2", HttpMethod.Post, contentPhase3, jwt: phase2Content.Token);

    RequestResult requestResultPhase3 = await RequestResult.FromApiResult(resultPhase3);
    if (!requestResultPhase3.IsSuccess)
      return requestResultPhase3;

    // Phase 5
    UserControllerRecords.SrpStep4Response? phase4Content = await resultPhase3.Response!.Content.ReadFromJsonAsync<UserControllerRecords.SrpStep4Response>();

    if (phase4Content == null)
      return RequestResult.Failure("Error: Server replied without a content", resultPhase3);

    try
    {
      srpClient.VerifySession(clientEphemeral.Public, clientSession, phase4Content.Proof);
    }
    catch (SecurityException)
    {
      return RequestResult.Failure("Error: failed to verify the server session.", null);
    }

    info.SetJWTToStorage(phase4Content.TokenPair);
    return RequestResult.Success(resultPhase3);
  }

  public async Task<RequestResult> Register(string username, string visibleName, string password)
  {
    HttpContent content = GenerateCreateRequest(username, visibleName, password);

    ApiResult result = await api.SendRequestAsync("user/create", HttpMethod.Post, content);

    RequestResult requestResult = await RequestResult.FromApiResult(result);

    if (!result.IsSuccess)
      return requestResult;

    UserControllerRecords.CreateResult? response = await result.Response!.Content.ReadFromJsonAsync<UserControllerRecords.CreateResult>();
    if (response == null)
      return RequestResult.Failure("Error: failed to serialize the response content", null);

    // NOTE: Additional user info can be saved from the response for runtime storage

    info.SetJWTToStorage(response.TokenPair);
    return RequestResult.Success(result);
  }

  private HttpContent GenerateCreateRequest(string username, string password, string visibleName)
  {
    string formattedUsername = username.ToLower();
    SrpClient srpClient = new();
    string salt = srpClient.GenerateSalt();
    string privateKey = srpClient.DerivePrivateKey(salt, formattedUsername, password);
    string verifier = srpClient.DeriveVerifier(privateKey);

    UserControllerRecords.CreateUserRequest contentRecord = new(formattedUsername,
        visibleName, salt, verifier);

    return JsonContent.Create(contentRecord);
  }
}
