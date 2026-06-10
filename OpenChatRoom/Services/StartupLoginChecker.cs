using Microsoft.AspNetCore.Components;

public abstract class StartupLoginChecker
{
  public static async Task CheckLoginAsync(CheckRequestHandler check, LoginInfoManager infoManager, ApiClient apiClient, NavigationManager navigationManager, AppConfig config)
  {
    string? server = infoManager.GetServerFromStorage();
    if (string.IsNullOrWhiteSpace(server))
      server = config.ApiUrl;
    if (string.IsNullOrWhiteSpace(server))
    {
      navigationManager.NavigateTo("connect");
      return;
    }
    if (!apiClient.setBaseAddress(server))
      navigationManager.NavigateTo("connect");
    else
    {
      RequestResult result = await check.CheckAuth();
      if (result.ApiResult != null)
        infoManager.RedirectFromApiResult(result.ApiResult);
      else
        navigationManager.NavigateTo("login");
    }
  }
}
