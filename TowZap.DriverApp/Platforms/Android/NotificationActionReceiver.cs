using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Services;

namespace TowZap.DriverApp.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "ACCEPT_JOB", "REJECT_JOB" })]
    public class NotificationActionReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            var jobId = intent.GetStringExtra("JobId");

            if (string.IsNullOrEmpty(jobId))
                return;

            Toast.MakeText(context, $"Action: {action} on Job {jobId}", ToastLength.Short).Show();

            var jobService = ServiceHelper.GetService<JobService>();

            if (action == "ACCEPT_JOB")
                await jobService.UpdateJobStatusAsync(Guid.Parse(jobId), Enums.JobStatus.EnRoute);
            else if (action == "REJECT_JOB")
                await jobService.UpdateJobStatusAsync(Guid.Parse(jobId), Enums.JobStatus.Cancelled);
        }
    }
}
