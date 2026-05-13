using Microsoft.AspNetCore.Components;

public abstract class StartupLoginChecker
{
  public static async Task CheckLoginAsync(CheckRequestHandler check, LoginInfoManager infoManager, ApiClient apiClient, NavigationManager navigationManager)
  {
    string? server = infoManager.GetServerFromStorage();
    if (string.IsNullOrWhiteSpace(server))
    {
      navigationManager.NavigateTo("connect");
      return;
    }
    apiClient.setBaseAddress(server);
    await check.CheckAuth();
  }
}
