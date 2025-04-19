using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Services;
using UserNotifications;

namespace TowZap.DriverApp.Platforms.iOS
{
    public class NotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override async void DidReceiveNotificationResponse(UNUserNotificationCenter center,
            UNNotificationResponse response,
            Action completionHandler)
        {
            var jobIdStr = response.Notification.Request.Content.UserInfo?["JobId"]?.ToString();

            if (!string.IsNullOrWhiteSpace(jobIdStr) && Guid.TryParse(jobIdStr, out var jobId))
            {
                var jobService = ServiceHelper.GetService<JobService>();

                if (response.ActionIdentifier == "ACCEPT_JOB")
                    await jobService.UpdateJobStatusAsync(jobId, JobStatus.EnRoute);
                else if (response.ActionIdentifier == "REJECT_JOB")
                    await jobService.UpdateJobStatusAsync(jobId, JobStatus.Cancelled);
            }

            completionHandler?.Invoke();
        }
    }
}
