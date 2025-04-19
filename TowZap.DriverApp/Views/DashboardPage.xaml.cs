using Microsoft.Maui.Devices.Sensors;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;

namespace TowZap.DriverApp.Views;
public partial class DashboardPage : BaseShellPage<DashboardViewModel>
{
    public DashboardPage() : base(new DashboardViewModel(
        ServiceHelper.GetService<JobService>(),
        ServiceHelper.GetService<SignalRClientService>(),
        ServiceHelper.GetService<GeocodingService>(),
        ServiceHelper.GetService<SessionManager>()))
    {
        InitializeComponent();
        Title = "Dashboard";
    }
    private void OnJobAccepted(object sender, Guid jobId)
    {
        ViewModel.AcceptJobCommand.Execute(null); 
    }

    private void OnJobRejected(object sender, Guid jobId)
    {
        ViewModel.DeclineJobCommand.Execute(null);
    }

}