using TowZap.DriverApp.Views;

namespace TowZap.DriverApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("JobDetailPage", typeof(JobDetailPage)); 
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));

        }
    }
}
