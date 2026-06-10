public abstract class AuthorizationRequestHandlerBase(ApiClient api, LoginInfoManager manager)
{
  protected ApiClient apiClient { get; } = api;

  private readonly LoginInfoManager infoManager = manager;

  protected async Task<RequestResult> TryRequestWithJWTAsync(string endpoint, HttpMethod method, HttpContent? content = null)
  {
    string? jwt = infoManager.GetJWTFromStorage();
    ApiResult result = await apiClient.SendRequestAsync(endpoint, method, jwt: jwt, content: content);

    if (result.IsSuccess && result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
      // jwt = infoManager.GetRefreshJWTFromStorage();
      return await UpdateJWTThenRetryAsync(endpoint, method, jwt, content);
    }
    return RequestResult.FromApiResult(result);
  }

  private async Task<RequestResult> UpdateJWTThenRetryAsync(string endpoint, HttpMethod method, string? refreshJwt, HttpContent? content = null)
  {
    ApiResult result = await apiClient.SendRequestAsync("check/refresh", HttpMethod.Get, jwt: refreshJwt);
    RequestResult requestResult = RequestResult.FromApiResult(result);

    if (!requestResult.IsSuccess)
    {
      return requestResult;
    }

    string newJwt = await result.Response!.Content.ReadAsStringAsync();
    if (newJwt.IsWhiteSpace())
    {
      return RequestResult.Failure("Server Failed to output JWT during update", null);
    }

    infoManager.SetJWTToStorage(newJwt);

    ApiResult finalResult = await apiClient.SendRequestAsync(endpoint, method, content, newJwt);
    return RequestResult.FromApiResult(finalResult);
  }

}

