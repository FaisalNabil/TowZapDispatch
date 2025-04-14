using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TowZap.Client.Client;
using TowZap.Client.Client.Models;
using TowZap.Client.Client.Service;
using TowZap.Client.Client.Service.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

string env = builder.HostEnvironment.Environment; // "Development", "Production", etc.
string configPath = env == "Production" ? "appsettings.Production.json" : "appsettings.json";

var configJson = await http.GetStringAsync(configPath);
var config = JsonSerializer.Deserialize<Dictionary<string, ApiSettings>>(configJson);
var apiSettings = config["ApiSettings"];

// Register ApiSettings as singleton
builder.Services.AddSingleton(apiSettings);

// Register Auth Handler
builder.Services.AddTransient<AuthHttpMessageHandler>();

// Named HttpClient with handler
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
}).AddHttpMessageHandler<AuthHttpMessageHandler>();

// Use factory for typed services
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiClient");
});

// Add services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<ExpiryService>();
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IClientUserService, ClientUserService>(); 
builder.Services.AddScoped<IClientJobRequestService, ClientJobRequestService>();

builder.Services.AddScoped<JobUpdateService>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddSingleton<SignalRService>();



await builder.Build().RunAsync();
