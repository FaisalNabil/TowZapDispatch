using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace TowZap.DriverApp.Helper
{
    public static class NotificationHelper
    {
        public static void Show(string title, string body)
        {
#if ANDROID
        var context = Android.App.Application.Context;

        var builder = new Android.App.Notification.Builder(context, "default")
            .SetContentTitle(title)
            .SetContentText(body)
            .SetSmallIcon(Microsoft.Maui.Resource.Drawable.ic_notification) // replace with your app icon
            .SetAutoCancel(true);

        var notification = builder.Build();
        var notificationManager = Android.App.NotificationManager.FromContext(context);
        notificationManager.Notify(1001, notification);
#elif IOS
        var content = new UserNotifications.UNMutableNotificationContent
        {
            Title = title,
            Body = body
        };

        var trigger = UserNotifications.UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
        var request = UserNotifications.UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);
        UserNotifications.UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
#elif WINDOWS
            var toast = new Windows.UI.Notifications.ToastNotification(
                new Windows.Data.Xml.Dom.XmlDocument());
            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(toast);
#endif
        }
    }
}
