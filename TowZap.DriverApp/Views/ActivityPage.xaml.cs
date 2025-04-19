using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views;

public partial class ActivityPage : BaseShellPage<ActivityViewModel>
{
    public ActivityPage()
        : base(new ActivityViewModel(
            ServiceHelper.GetService<JobService>(),
            ServiceHelper.GetService<SessionManager>()))
    {
        InitializeComponent();
        Title = "Activity";
    }
}
