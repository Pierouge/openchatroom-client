using Microsoft.AspNetCore.Components;

public abstract class AuthorizationRequestHandlerBase(ApiClient api, LoginInfoManager manager, NavigationManager nav)
{
  protected ApiClient apiClient { get; } = api;

  private readonly LoginInfoManager infoManager = manager;

  private readonly NavigationManager navigationManager = nav;

  protected async Task<RequestResult> TryRequestWithJWTAsync(string endpoint, HttpMethod method, HttpContent? content = null)
  {
    string? jwt = infoManager.GetJWTFromStorage();
    ApiResult result = await apiClient.SendRequestAsync(endpoint, method, jwt: jwt, content: content);

    if (result.IsSuccess && result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized)
      return await UpdateJWTThenRetryAsync(endpoint, method, jwt, content);

    RedirectFromApiResult(result);

    return RequestResult.FromApiResult(result);
  }

  private async Task<RequestResult> UpdateJWTThenRetryAsync(string endpoint, HttpMethod method, string? jwt, HttpContent? content = null)
  {
    ApiResult result = await apiClient.SendRequestAsync("check/refresh", HttpMethod.Get, jwt: jwt);
    RequestResult requestResult = RequestResult.FromApiResult(result);

    if (!requestResult.IsSuccess)
    {
      RedirectFromApiResult(result);
      return requestResult;
    }

    string newJwt = await result.Response!.Content.ReadAsStringAsync();
    if (newJwt.IsWhiteSpace())
    {
      navigationManager.NavigateTo("login");
      return RequestResult.Failure("Server Failed to output JWT during update", null);
    }

    infoManager.SetJWTToStorage(newJwt);

    ApiResult finalResult = await apiClient.SendRequestAsync(endpoint, method, content, newJwt);
    return RequestResult.FromApiResult(finalResult);
  }

  private void RedirectFromApiResult(ApiResult result)
  {
    if (!result.IsSuccess)
      navigationManager.NavigateTo("connect");
    else if (result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized)
      navigationManager.NavigateTo("login");
  }
}

