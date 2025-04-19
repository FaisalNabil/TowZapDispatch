using TowZap.DriverApp.Services;
using TowZap.DriverApp.Views;

namespace TowZap.DriverApp
{
    public partial class App : Application
    {
        private readonly SessionManager _session;

        public App(SessionManager sessionManager)
        {
            InitializeComponent();
            _session = sessionManager;

            // Temporarily show a loading/splash page while we check session
            MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            // Defer navigation until UI is ready
            Task.Run(async () => await SetupInitialPageAsync());
        }

        private async Task SetupInitialPageAsync()
        {
            await _session.InitializeAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (_session.IsLoggedIn)
                    MainPage = new MainShell();
                else
                    MainPage = new LoginShell();
            });
        }
    }
}
