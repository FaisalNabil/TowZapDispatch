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
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register HttpClient with BaseAddress from AppSettings
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri(AppSettings.ApiBaseUrl);
            })
            #if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
             {
                 ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Disable SSL validation
             });
            #else
            ;
            #endif

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
