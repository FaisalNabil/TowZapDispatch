using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.ViewModels;
using TowZap.DriverApp.Views;

namespace TowZap.DriverApp
{
    public partial class MainShell : Shell
    {
        public MainShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("JobDetailPage", typeof(JobDetailPage)); 
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));

            var signalR = ServiceHelper.GetService<SignalRClientService>();
            var jobService = ServiceHelper.GetService<JobService>();
            var session = ServiceHelper.GetService<SessionManager>();

            var token = session.Token;
            var hubUrl = $"{ConfigurationService.Get("SignalRHubUrl")}hubs/jobUpdates";

            signalR.InitializeAsync(hubUrl, token);

            signalR.On<Guid>("JobCreated", async (jobId) =>
            {
                var jobService = ServiceHelper.GetService<JobService>();
                var session = ServiceHelper.GetService<SessionManager>();
                var job = await jobService.GetJobByIdAsync(jobId);

                if (job != null && job.AssignedDriverId == session.UserId)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (Shell.Current.CurrentPage?.BindingContext is DashboardViewModel vm)
                        {
                            vm.ReceiveJobNotification(job);
                        }
                    });
                }
            });


        }
    }
}
