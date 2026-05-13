using Microsoft.AspNetCore.Components;

public class LoginScreenRedirector
{
  public LoginScreenRedirector(AppInitializationService init, NavigationManager nav)
  {
    init.OnInitialized += async () =>
    {
      // TODO: add logic for app redirection
      nav.NavigateTo("app");
    };
  }
}
