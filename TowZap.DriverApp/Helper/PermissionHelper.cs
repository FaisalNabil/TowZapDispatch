using System.Threading.Tasks;

namespace TowZap.DriverApp.Helper
{
    public static class PermissionHelper
    {
        public static async Task<bool> RequestNotificationPermissionAsync()
        {
#if ANDROID
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();

            return status == PermissionStatus.Granted;
#elif IOS
            // No explicit prompt needed; handled by OS.
            return true;
#else
            return true;
#endif
        }
    }
}
