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
            MainPage = new AppShell(); // always load Shell first
        }

        protected override async void OnStart()
        {
            await _session.InitializeAsync();

            // Redirect to login if no token
            if (!_session.IsLoggedIn)
            {
                await Shell.Current.GoToAsync("LoginPage");
            }
        }

        // Optional: If you want to check on resume too
        protected override async void OnResume()
        {
            await _session.InitializeAsync();

            if (!_session.IsLoggedIn)
            {
                await Shell.Current.GoToAsync("LoginPage");
            }
        }
    }
}
