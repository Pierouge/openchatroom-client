using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenChatRoom;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add a apiClient for the api Requests
builder.Services.AddScoped<ApiClient>();

// Add Local and Session Storage Components
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();



await builder.Build().RunAsync();
