using System.Net.Http.Json;
using System.Security;
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

    // Phase 1
    Dictionary<string, string> phase1Dict = new(){
      {"username", username},
      {"client_public_ephemeral", clientEphemeral.Public}
    };

    HttpContent contentPhase1 = JsonContent.Create(phase1Dict);

    ApiResult resultPhase1 = await api.SendRequestAsync("user/srp/1", HttpMethod.Post, contentPhase1);

    if (!resultPhase1.IsSuccess) return RequestResult.Failure(resultPhase1.Exception!.Message, resultPhase1);

    if (!resultPhase1.Response!.IsSuccessStatusCode)
      return RequestResult.Failure(await resultPhase1.Response!.Content.ReadAsStringAsync(), resultPhase1);

    // Phase 3
    string contentPhase2string = await resultPhase1.Response!.Content.ReadAsStringAsync();
    Dictionary<string, string>? phase2Dict = JsonSerializer.Deserialize<Dictionary<string, string>>(contentPhase2string);

    if (phase2Dict == null)
      return RequestResult.Failure("Error: Server replied without a content", resultPhase1);

    string serverPublicEphemeral = phase2Dict["server_public_ephemeral"];
    string salt = phase2Dict["salt"];
    string token = phase2Dict["token"];

    Console.WriteLine(string.Concat("Token: ", token));

    if (string.IsNullOrWhiteSpace(serverPublicEphemeral) || string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(token))
      return RequestResult.Failure("Error: Server replied with incomplete content", resultPhase1);

    string privateKey = srpClient.DerivePrivateKey(salt, username, password);
    SrpSession clientSession = srpClient.DeriveSession(clientEphemeral.Secret, serverPublicEphemeral, salt, username, privateKey);

    Dictionary<string, string> phase3Dict = new(){
      {"proof", clientSession.Proof}
    };
    HttpContent contentPhase3 = JsonContent.Create(phase3Dict);

    ApiResult resultPhase3 = await api.SendRequestAsync("user/srp/2", HttpMethod.Post, contentPhase3, jwt: token);

    if (!resultPhase3.IsSuccess) return RequestResult.Failure(resultPhase3.Exception!.Message, resultPhase3);

    if (!resultPhase3.Response!.IsSuccessStatusCode)
      return RequestResult.Failure(await resultPhase3.Response!.Content.ReadAsStringAsync(), resultPhase3);

    // Phase 5
    string contentPhase4string = await resultPhase3.Response!.Content.ReadAsStringAsync();
    Dictionary<string, string>? phase4Dict = JsonSerializer.Deserialize<Dictionary<string, string>>(contentPhase4string);

    if (phase4Dict == null)
      return RequestResult.Failure("Error: Server replied without a content", resultPhase3);

    string serverProof = phase4Dict["proof"];
    token = phase4Dict["token"];

    if (string.IsNullOrWhiteSpace(serverProof) || string.IsNullOrWhiteSpace(token))
      return RequestResult.Failure("Error: Server replied with incomplete content", resultPhase3);

    try
    {
      srpClient.VerifySession(clientEphemeral.Public, clientSession, serverProof);
    }
    catch (SecurityException)
    {
      return RequestResult.Failure("Error: failed to verify the server session.", null);
    }

    info.SetJWTToStorage(token);
    return RequestResult.Success(null);
  }

  public async Task<RequestResult> Register(string username, string visibleName, string password)
  {
    string formattedUsername = username.ToLower();

    SrpClient srpClient = new();
    string salt = srpClient.GenerateSalt();
    string privateKey = srpClient.DerivePrivateKey(salt, formattedUsername, password);
    string verifier = srpClient.DeriveVerifier(privateKey);

    Dictionary<string, string> contentDict = new(){
      {"username", formattedUsername},
      {"visibleName", visibleName},
      {"salt", salt},
      {"verifier", verifier}
    };

    HttpContent content = JsonContent.Create(contentDict);

    ApiResult result = await api.SendRequestAsync("user/create", HttpMethod.Post, content);

    if (!result.IsSuccess)
      return RequestResult.Failure(result.Exception!.Message, result);

    string responseString = await result.Response!.Content.ReadAsStringAsync();

    if (!result.Response!.IsSuccessStatusCode)
      return RequestResult.Failure(responseString, result);

    Dictionary<string, object>? response = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString);
    if (response == null)
      return RequestResult.Failure("Error: failed to serialize the response content", null);

    // NOTE: Additional user info can be saved from the response for runtime storage

    string? token = response["token"].ToString();
    if (string.IsNullOrWhiteSpace(token))
      return RequestResult.Failure("Error: failed to fetch the JWT", null);

    info.SetJWTToStorage(token);
    return RequestResult.Success(null);
  }
}
