namespace TowZap.DriverApp.Views;

using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;
public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();

        var jobService = ServiceHelper.GetService<JobService>();
        var geocodingService = ServiceHelper.GetService<GeocodingService>();
        var signalRService = ServiceHelper.GetService<SignalRClientService>();
        var sessionManager = ServiceHelper.GetService<SessionManager>();

        BindingContext = new DashboardViewModel(jobService, signalRService, geocodingService, sessionManager);
    }
}