public class CheckRequestHandler(ApiClient api, LoginInfoManager info)
{
  private readonly ApiClient apiClient = api;
  private readonly LoginInfoManager infoManager = info;

  public async Task<RequestResult> Check()
  {
    ApiResult result = await apiClient.SendRequestAsync("check", HttpMethod.Get);

    if (!result.IsSuccess) return RequestResult.Failure(result.Exception!.Message, result);

    if (result.Response!.IsSuccessStatusCode) return RequestResult.Success(result);
    return RequestResult.Failure(result.Response!.Content.ToString(), result);
  }

  public async Task<RequestResult> CheckAuth()
  {
    string? jwt = infoManager.GetJWTFromStorage();
    ApiResult result = await apiClient.SendRequestAsync("check", HttpMethod.Get, jwt: jwt);

    infoManager.RedirectFromApiResult(result);

    if (!result.IsSuccess) return RequestResult.Failure(result.Exception!.Message, result);

    if (result.Response!.IsSuccessStatusCode) return RequestResult.Success(result);
    return RequestResult.Failure(result.Response!.Content.ToString(), result);
  }
}
