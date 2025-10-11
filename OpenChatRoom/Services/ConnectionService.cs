using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

public static class ConnectionService
{
    public enum loginStates
    {
        noServer,
        server,
        credentials
    }
    public static async Task<loginStates> checkLogin(ISyncSessionStorageService sessionStorage,
     ISyncLocalStorageService localStorage, ApiClient apiClient)
    {
        try
        {
            // Check if the server is stored in session or local storage
            string? storedServer = sessionStorage.GetItem<string?>("server");
            string? storedUsername = sessionStorage.GetItem<string?>("username");
            if (string.IsNullOrEmpty(storedServer)) storedServer = localStorage.GetItem<string>("server");
            if (string.IsNullOrEmpty(storedUsername)) storedUsername = localStorage.GetItem<string>("username");
            if (string.IsNullOrEmpty(storedServer)) return loginStates.noServer;
            else
            {
                string endpoint = string.Empty;
                if (string.IsNullOrEmpty(storedUsername)) endpoint = "check";
                else endpoint = string.Concat("check", "/", storedUsername);

                apiClient.setBaseAddress(storedServer); // Update the server
                HttpResponseMessage httpResponse = await apiClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(storedUsername)) return loginStates.server;
                    return loginStates.credentials;
                }
                
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return loginStates.server;

                return loginStates.noServer;
            }
        }
        catch (HttpRequestException)
        {
            return loginStates.noServer;
        }
        catch (AggregateException aggrEx)
        {
            foreach (var innerEx in aggrEx.Flatten().InnerExceptions)
            {
                if (innerEx is HttpRequestException httpEx)
                {
                    return loginStates.noServer;
                }
                else if (!(innerEx is NavigationException)) throw;
            }
        }
        return loginStates.noServer;
    }
}