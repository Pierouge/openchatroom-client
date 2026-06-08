using Microsoft.AspNetCore.Components;

public class CheckRequestHandler(ApiClient api, LoginInfoManager manager, NavigationManager nav) : AuthorizationRequestHandlerBase(api, manager, nav)
{

  public async Task<RequestResult> Check()
  {
    ApiResult result = await apiClient.SendRequestAsync("check", HttpMethod.Get);
    return RequestResult.FromApiResult(result);
  }

  public async Task<RequestResult> CheckAuth()
  {
    return await TryRequestWithJWTAsync("check/auth", HttpMethod.Get);
  }
}
