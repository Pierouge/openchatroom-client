using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

public static class ConnectionService
{
    public static async Task<bool> checkConnection(ISyncSessionStorageService sessionStorage,
     ISyncLocalStorageService localStorage, ApiClient apiClient)
    {
        try
        {
            // Check if the server is stored in session or local storage
            string? storedServer = sessionStorage.GetItem<string?>("server");
            if (string.IsNullOrEmpty(storedServer)) storedServer = localStorage.GetItem<string>("server");
            if (string.IsNullOrEmpty(storedServer)) return false;
            else
            {
                apiClient.setBaseAddress(storedServer); // Update the server
                HttpResponseMessage httpResponse = await apiClient.GetAsync("check");

                if (httpResponse.IsSuccessStatusCode) return true;
                else return false;
            }
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (AggregateException aggrEx)
        {
            foreach (var innerEx in aggrEx.Flatten().InnerExceptions)
            {
                if (innerEx is HttpRequestException httpEx)
                {
                    return false;
                }
                else if (!(innerEx is NavigationException)) throw;
            }
        }
        return false;
    }
    public static async Task<bool> checkLogin(ISyncSessionStorageService sessionStorage,
     ISyncLocalStorageService localStorage, ApiClient apiClient)
    {
        try
        {
            // Check if the server is stored in session or local storage
            string? storedServer = sessionStorage.GetItem<string?>("server");
            string? storedUsername = sessionStorage.GetItem<string?>("username");
            if (string.IsNullOrEmpty(storedServer)) storedServer = localStorage.GetItem<string>("server");
            if (string.IsNullOrEmpty(storedUsername)) storedUsername = localStorage.GetItem<string>("username");
            if (string.IsNullOrEmpty(storedUsername)) return false;
            if (string.IsNullOrEmpty(storedServer)) return false;
            else
            {
                apiClient.setBaseAddress(storedServer); // Update the server
                string endpoint = string.Concat("check", "/", storedUsername);
                HttpResponseMessage httpResponse = await apiClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode) return true;
                else return false;
            }
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (AggregateException aggrEx)
        {
            foreach (var innerEx in aggrEx.Flatten().InnerExceptions)
            {
                if (innerEx is HttpRequestException httpEx)
                {
                    return false;
                }
                else if (!(innerEx is NavigationException)) throw;
            }
        }
        return false;
    }
}