using System.Net.Http.Json;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenChatRoom;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load runtime-patched config from wwwroot
HttpClient http = new() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
AppConfig config = await http.GetFromJsonAsync<AppConfig>("appsettings.json") ?? throw new Exception("Failed to load appsettings.json");

// Register immutable config
builder.Services.AddSingleton(config);

// Register HttpClient
// builder.Services.AddScoped(sp => http);

// Add Local and Session Storage Components
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();

// Add the ApiClient as a Singleton
builder.Services.AddSingleton<ApiClient>();

// Add the LoginInfoManager as a Singleton
builder.Services.AddSingleton<LoginInfoManager>();

// Add Handlers as Singletons
builder.Services.AddSingleton<CheckRequestHandler>();
builder.Services.AddSingleton<LoginRequestHandler>();

await builder.Build().RunAsync();
