using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

public class LoginInfoManager(ISyncLocalStorageService storageService, NavigationManager nav)
{
  private readonly ISyncLocalStorageService localStorage = storageService;
  private readonly NavigationManager navigationManager = nav;

  public void SetJWTToStorage(string jwt)
  {
    localStorage.SetItemAsString("token", jwt);
  }

  public string? GetJWTFromStorage()
  {
    return localStorage.GetItemAsString("token");
  }

  public void SetRefreshJWTToStorage(string jwt)
  {
    localStorage.SetItemAsString("refresh_token", jwt);
  }

  public string? GetRefreshJWTFromStorage()
  {
    return localStorage.GetItemAsString("refresh_token");
  }

  public void SetServerToStorage(string server)
  {
    localStorage.SetItemAsString("server", server);
  }

  public string? GetServerFromStorage()
  {
    return localStorage.GetItemAsString("server");
  }

  public void RedirectFromApiResult(ApiResult result)
  {
    if (!result.IsSuccess)
      navigationManager.NavigateTo("connect");
    else if (result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized)
      navigationManager.NavigateTo("login");
  }

}
