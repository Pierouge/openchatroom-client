public record AppConfig
{
  public string? ApiUrl { get; init; }

  public AppConfig(string apiUrl)
  {
    ApiUrl = apiUrl;
  }

  public AppConfig() { }
}
