namespace TowZap.DriverApp.Views;

using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;
public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();

        var jobService = ServiceHelper.GetService<JobService>();
        var signalRService = ServiceHelper.GetService<SignalRClientService>();

        BindingContext = new DashboardViewModel(jobService, signalRService);
    }
}