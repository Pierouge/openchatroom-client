using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

public class LoginInfoManager(NavigationManager nav, ISyncLocalStorageService storageService)
{
  private readonly NavigationManager navigationManager = nav;
  private readonly ISyncLocalStorageService localStorage = storageService;

  public void SetJWTToStorage(string jwt)
  {
    localStorage.SetItem("token", jwt);
  }

  public string? GetJWTFromStorage()
  {
    return localStorage.GetItemAsString("token");
  }

  public void SetServerToStorage(string server)
  {
    localStorage.SetItem("server", server);
  }

  public string? GetServerFromStorage()
  {
    return localStorage.GetItemAsString("server");
  }

  public bool RedirectFromApiResult(ApiResult result)
  {
    if (!result.IsSuccess)
    {
      navigationManager.NavigateTo("connect");
      return true;
    }
    else if (result.Response!.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
      navigationManager.NavigateTo("login");
      return true;
    }
    return false;
  }
}
