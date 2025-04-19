using Microsoft.Maui.Devices.Sensors;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TowZap.DriverApp.Services
{
    public class LocationTrackingService
    {
        private readonly SignalRClientService _signalR;
        private Timer _timer;
        private string _jobId;

        public bool IsTracking { get; private set; }

        public LocationTrackingService(SignalRClientService signalR)
        {
            _signalR = signalR;
        }

        public void StartTracking(string jobId, double intervalSeconds = 10)
        {
            if (IsTracking) return;

            _jobId = jobId;
            _timer = new Timer(intervalSeconds * 1000);
            _timer.Elapsed += async (s, e) => await SendLocationAsync();
            _timer.AutoReset = true;
            _timer.Start();

            IsTracking = true;
        }

        public void StopTracking()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            IsTracking = false;
        }

        private async Task SendLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync()
                               ?? await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

                if (location != null && _signalR.IsConnected)
                {
                    await _signalR.SendAsync("SendLocationUpdate", _jobId, location.Latitude, location.Longitude);
                    Console.WriteLine($"📡 Sent location: {location.Latitude}, {location.Longitude}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Location send failed: {ex.Message}");
            }
        }
    }
}
