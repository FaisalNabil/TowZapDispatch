using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using TowZap.Client.Client;
using TowZap.Client.Client.Models;
using TowZap.Client.Client.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configJson = await http.GetStringAsync("appsettings.json");
var config = JsonSerializer.Deserialize<Dictionary<string, ApiSettings>>(configJson);
var apiSettings = config["ApiSettings"];

// Register ApiSettings as singleton
builder.Services.AddSingleton(apiSettings);

// Use it to set HttpClient base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiSettings.BaseUrl) });

// Add services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IClientUserService, ClientUserService>();


builder.Services.AddScoped<UserContext>();
builder.Services.AddSingleton<SignalRService>();



await builder.Build().RunAsync();
