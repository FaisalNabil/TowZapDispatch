using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using TowZap.DriverApp.Enums;
#if ANDROID
using TowZap.DriverApp.Platforms.Android;
#elif IOS
#elif WINDOWS
#endif

namespace TowZap.DriverApp.Helper
{
    public static class NotificationHelper
    {
        public static void Show(string title, string body, NotificationType type = NotificationType.Info, Guid? jobId = null)
        {
#if ANDROID
            ShowAndroidNotification(title, body, type, jobId);
#elif IOS
            ShowiOSNotification(title, body, type, jobId);
#elif WINDOWS
            ShowWindowsNotification(title, body, type, jobId);
#endif
        }

#if ANDROID
        [System.Runtime.Versioning.SupportedOSPlatform("android")]
        private static void ShowAndroidNotification(string title, string body, NotificationType type, Guid? jobId)
        {
            var context = Android.App.Application.Context;
            var channelId = "default";

            var channel = new Android.App.NotificationChannel(channelId, "TowZap Notifications", Android.App.NotificationImportance.High)
            {
                Description = "Tow job notifications"
            };

            var notificationManager = (Android.App.NotificationManager)context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            var builder = new Android.App.Notification.Builder(context, channelId)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetSmallIcon(Microsoft.Maui.Resource.Drawable.ic_notification)
                .SetAutoCancel(true);

            if (type == NotificationType.JobWithActions && jobId.HasValue)
            {
                var acceptIntent = new Android.Content.Intent(context, typeof(NotificationActionReceiver));
                acceptIntent.SetAction("ACCEPT_JOB");
                acceptIntent.PutExtra("JobId", jobId.ToString());

                var rejectIntent = new Android.Content.Intent(context, typeof(NotificationActionReceiver));
                rejectIntent.SetAction("REJECT_JOB");
                rejectIntent.PutExtra("JobId", jobId.ToString());

                var acceptPending = Android.App.PendingIntent.GetBroadcast(context, 0, acceptIntent, Android.App.PendingIntentFlags.Immutable);
                var rejectPending = Android.App.PendingIntent.GetBroadcast(context, 1, rejectIntent, Android.App.PendingIntentFlags.Immutable);

                builder.AddAction(new Android.App.Notification.Action.Builder(null, "Accept", acceptPending).Build());
                builder.AddAction(new Android.App.Notification.Action.Builder(null, "Reject", rejectPending).Build());
            }

            var notification = builder.Build();
            notificationManager.Notify(1001, notification);
        }
#endif

#if IOS
        [System.Runtime.Versioning.SupportedOSPlatform("ios")]
        private static void ShowiOSNotification(string title, string body, NotificationType type, Guid? jobId)
        {
            var content = new UserNotifications.UNMutableNotificationContent
            {
                Title = title,
                Body = body
            };

            // To support Accept/Reject, you must define UNNotificationCategory and add actions in AppDelegate
            if (type == NotificationType.JobWithActions)
                {content.CategoryIdentifier = "TOW_JOB_ACTIONS"; // Needs to be registered in iOS project
                content.UserInfo = new Foundation.NSDictionary("JobId", jobId.ToString());}

            var trigger = UserNotifications.UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
            var request = UserNotifications.UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);
            UserNotifications.UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
        }
#endif

#if WINDOWS
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void ShowWindowsNotification(string title, string body, NotificationType type, Guid? jobId)
        {
            // Optional: Add Windows toast implementation
        }
#endif
    }
}
