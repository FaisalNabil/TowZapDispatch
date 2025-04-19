using TowZap.DriverApp.Views;

namespace TowZap.DriverApp
{
    public partial class LoginShell : Shell
    {
        public LoginShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("DashboardPage", typeof(DashboardPage));

            Routing.RegisterRoute("JobDetailPage", typeof(JobDetailPage)); 
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));

        }
    }
}
