public class CheckRequestHandler(ApiClient api, LoginInfoManager manager) : AuthorizationRequestHandlerBase(api, manager)
{
  public async Task<RequestResult> Check()
  {
    ApiResult result = await ApiClient.SendRequestAsync("check", HttpMethod.Get);
    return RequestResult.FromApiResult(result);
  }

  public async Task<RequestResult> CheckAuth()
  {
    return await TryRequestWithJWTAsync("check/auth", HttpMethod.Get);
  }
}
