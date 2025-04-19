using Microsoft.Extensions.Logging;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Config;
using TowZap.DriverApp.Views;
using TowZap.DriverApp.ViewModels;

#if DEBUG
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif


namespace TowZap.DriverApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Load config file
            Task.Run(async () => await ConfigurationService.InitializeAsync()).Wait();

            var baseUrl = ConfigurationService.Get("ApiBaseUrl");

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.UseMauiMaps();

            builder.Services.AddSingleton<HttpClient>(sp =>
            {
#if DEBUG
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(ConfigurationService.Get("ApiBaseUrl")),
                    Timeout = TimeSpan.FromSeconds(30)
                };
#else
    return new HttpClient
    {
        BaseAddress = new Uri(ConfigurationService.Get("ApiBaseUrl")),
        Timeout = TimeSpan.FromSeconds(30)
    };
#endif
            });

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<JobService>();
            builder.Services.AddSingleton<GeocodingService>();
            builder.Services.AddSingleton<SignalRClientService>();
            builder.Services.AddSingleton<SessionManager>();
            builder.Services.AddSingleton<LocationTrackingService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<ActivityPage>();
            builder.Services.AddTransient<ActivityViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
