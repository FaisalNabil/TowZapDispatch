using TowZap.DriverApp.Views;

namespace TowZap.DriverApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
