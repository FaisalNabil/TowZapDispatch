using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views;

public partial class ActivityPage : ContentPage
{
	public ActivityPage()
	{
		InitializeComponent();

        var jobService = ServiceHelper.GetService<JobService>();
        var sessionManager = ServiceHelper.GetService<SessionManager>();

        BindingContext = new ActivityViewModel(jobService, sessionManager);
    }
}