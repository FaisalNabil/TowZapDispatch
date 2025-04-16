using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

        var userService = ServiceHelper.GetService<UserService>();
        var sessionManager = ServiceHelper.GetService<SessionManager>();

        BindingContext = new SettingsViewModel(userService, sessionManager);
    }
}