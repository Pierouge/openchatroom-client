using System.Text.RegularExpressions;

public class ApiClient
{
  private HttpClient? http;

  public string getBaseAddressString()
  {
    if (http == null) return string.Empty;
    return http.BaseAddress?.ToString() ?? string.Empty;
  }

  public void setBaseAddress(string baseUrl)
  {
    http = new HttpClient
    {
      BaseAddress = new Uri(filterAddress(baseUrl))
    };
  }

  public async Task<ApiResult> SendRequestAsync(string endpoint, HttpMethod method, HttpContent content)
  {
    if (http == null) throw new NullReferenceException(message: "Attempting to send a request to an unknown server");
    HttpRequestMessage request = new(method, endpoint)
    {
      Content = content
    };

    // Ensure works or gives the exception as a result
    try
    {
      return ApiResult.Success(await http.SendAsync(request));
    }
    catch (HttpRequestException ex)
    {
      return ApiResult.Failure(ex);
    }
  }

  public static string filterAddress(string serverIp)
  {
    string pattern = @"[^a-zA-Z0-9\.\-:\/_%~\+#\?&=@]";
    string filteredServer = Regex.Replace(serverIp, pattern, "");
    if (!filteredServer.EndsWith('/')) filteredServer = string.Concat(filteredServer, '/');
    if (filteredServer.Contains("http://") ||
        filteredServer.Contains("https://")) return filteredServer;
    else return string.Concat("https://", filteredServer);
  }
}

public class ApiResult
{
  public bool IsSuccess { get; }
  public HttpResponseMessage? Response { get; }
  public HttpRequestException? Exception { get; }

  private ApiResult(bool success, HttpResponseMessage? response, HttpRequestException? exception)
  {
    IsSuccess = success;
    Response = response;
    Exception = exception;
  }

  public static ApiResult Success(HttpResponseMessage response)
      => new(true, response, null);

  public static ApiResult Failure(HttpRequestException ex)
      => new(false, null, ex);
}

