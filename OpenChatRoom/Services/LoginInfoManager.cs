using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

public class LoginInfoManager(ISyncLocalStorageService storageService, NavigationManager nav)
{
  private readonly ISyncLocalStorageService localStorage = storageService;
  private readonly NavigationManager navigationManager = nav;

  public void SetJWTToStorage(TokenPair jwtPair)
  {
    localStorage.SetItemAsString("token", jwtPair.Token);
    localStorage.SetItemAsString("refreshToken", jwtPair.RefreshToken);
  }

  public TokenPair? GetJWTFromStorage()
  {
    string? jwt = localStorage.GetItemAsString("token");
    string? refreshJwt = localStorage.GetItemAsString("refreshToken");

    if (string.IsNullOrWhiteSpace(jwt) || string.IsNullOrWhiteSpace(refreshJwt))
      return null;

    return new TokenPair(jwt, refreshJwt);
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
