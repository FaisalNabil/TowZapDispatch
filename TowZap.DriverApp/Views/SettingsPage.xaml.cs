using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views;

public partial class SettingsPage : BaseShellPage<SettingsViewModel>
{
    public SettingsPage()
        : base(new SettingsViewModel(ServiceHelper.GetService<UserService>(),
                                     ServiceHelper.GetService<SessionManager>()))
    {
        InitializeComponent();
        Title = "Settings";
    }
}