using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}