using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

public class ApiClient()
{
    private HttpClient? httpClient;

    public string getBaseAddressString()
    {
        if (httpClient == null) return string.Empty;
        return httpClient.BaseAddress?.ToString() ?? string.Empty;
    }

    public void setBaseAddress(string baseUrl)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(filterAddress(baseUrl))
        };
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

    // To get the Cross Forgery Token
    public async Task getCsrfToken()
    {
        if (httpClient == null) throw new NullReferenceException();

        HttpResponseMessage responseMessage = await GetAsync("csrf/token");
        responseMessage.EnsureSuccessStatusCode();

        string json = await responseMessage.Content.ReadAsStringAsync();
        JsonDocument jsonDocument = JsonDocument.Parse(json);
        string? csrfToken = jsonDocument.RootElement.GetProperty("token").GetString();

        httpClient.DefaultRequestHeaders.Remove("OPENCHATROOM-CSRF-TOKEN");
        httpClient.DefaultRequestHeaders.Add("OPENCHATROOM-CSRF-TOKEN", csrfToken);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        if (httpClient == null) throw new NullReferenceException();
        HttpRequestMessage request = new(HttpMethod.Get, endpoint);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return await httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
    {
        if (httpClient == null) throw new NullReferenceException();
        HttpRequestMessage request = new(HttpMethod.Post, endpoint)
        {
            Content = content
        };

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return await httpClient.SendAsync(request);
    }
    
}