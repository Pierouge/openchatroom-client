using Blazored.LocalStorage;

public class LoginInfoManager(ISyncLocalStorageService storageService)
{
  private readonly ISyncLocalStorageService localStorage = storageService;

  public void SetJWTToStorage(string jwt)
  {
    localStorage.SetItemAsString("token", jwt);
  }

  public string? GetJWTFromStorage()
  {
    return localStorage.GetItemAsString("token");
  }

  public void SetServerToStorage(string server)
  {
    localStorage.SetItemAsString("server", server);
  }

  public string? GetServerFromStorage()
  {
    return localStorage.GetItemAsString("server");
  }


}
