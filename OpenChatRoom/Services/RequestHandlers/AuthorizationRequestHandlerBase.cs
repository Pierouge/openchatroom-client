using System.Net.Http.Json;

public abstract class AuthorizationRequestHandlerBase(ApiClient api, LoginInfoManager manager)
{
  protected ApiClient ApiClient { get; } = api;

  private readonly LoginInfoManager infoManager = manager;

  protected async Task<RequestResult> TryRequestWithJWTAsync(string endpoint, HttpMethod method, HttpContent? content = null)
  {
    TokenPair? jwtPair = infoManager.GetJWTFromStorage();

    ApiResult result = await ApiClient.SendRequestAsync(endpoint, method, jwt: jwtPair?.Token, content: content);

    if (result.IsSuccess && result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized && jwtPair != null)
      return await UpdateJWTThenRetryAsync(endpoint, method, jwtPair.RefreshToken, content);

    return await RequestResult.FromApiResult(result);
  }

  private async Task<RequestResult> UpdateJWTThenRetryAsync(string endpoint, HttpMethod method, string refreshJwt, HttpContent? content = null)
  {
    ApiResult result = await ApiClient.SendRequestAsync("check/refresh", HttpMethod.Get, jwt: refreshJwt);
    RequestResult requestResult = await RequestResult.FromApiResult(result);

    if (!requestResult.IsSuccess)
    {
      return requestResult;
    }

    TokenPair? newJWTPair = await result.Response!.Content.ReadFromJsonAsync<TokenPair>();
    if (newJWTPair == null)
    {
      return RequestResult.Failure("Server Failed to output JWT during update", null);
    }

    infoManager.SetJWTToStorage(newJWTPair);

    ApiResult finalResult = await ApiClient.SendRequestAsync(endpoint, method, content, newJWTPair.Token);
    return await RequestResult.FromApiResult(finalResult);
  }

}

