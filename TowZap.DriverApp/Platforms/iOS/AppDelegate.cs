using Foundation;
using TowZap.DriverApp.Platforms.iOS;
using UIKit;
using UserNotifications;

namespace TowZap.DriverApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Register notification actions
            UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

            var accept = UNNotificationAction.FromIdentifier("ACCEPT_JOB", "Accept", UNNotificationActionOptions.None);
            var reject = UNNotificationAction.FromIdentifier("REJECT_JOB", "Reject", UNNotificationActionOptions.Destructive);

            var category = UNNotificationCategory.FromIdentifier(
                "TOW_JOB_ACTIONS",
                new UNNotificationAction[] { accept, reject },
                new string[] { },
                UNNotificationCategoryOptions.None
            );

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(category));

            return base.FinishedLaunching(app, options);
        }
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
