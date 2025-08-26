using System.Text.RegularExpressions;

public class ApiClient
{
    private readonly HttpClient httpClient;

    public ApiClient(HttpClient http)
    {
        httpClient = http;
    }

    public string getBaseAddressString()
    {
        return httpClient.BaseAddress ? .ToString() ?? string.Empty;
    }

    public void setBaseAddress(string baseUrl)
    {
        httpClient.BaseAddress = new Uri(filterAddress(baseUrl));
    }

    public static string filterAddress(string serverIp)
    {
        string pattern = @"[^a-zA-Z0-9\.\-:\/_%~\+#\?&=@]";
        string filteredServer = Regex.Replace(serverIp, pattern, "");
        if (!filteredServer.EndsWith('/')) filteredServer = string.Concat(filteredServer, '/');
        if (filteredServer.Contains("http://") ||
            filteredServer.Contains("https://")) return filteredServer;
        else return string.Concat("http://", filteredServer);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return await httpClient.GetAsync(endpoint);
    }
}