using Microsoft.AspNetCore.SignalR.Client;
using TowZap.Client.Client.Models;

namespace TowZap.Client.Client.Service
{
    public class JobUpdateService
    {
        private readonly ApiSettings _config;
        private HubConnection _connection;

        public event Action<string> OnJobUpdated;
        public event Action<double, double> OnLocationUpdated;
        public JobUpdateService(ApiSettings config)
        {
            _config = config;
        }

        public async Task StartConnectionAsync(string jobId)
        {
            var hubUrl = _config.SignalRHubUrl;
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                _connection.On<string>("ReceiveJobUpdate", (message) =>
                {
                    OnJobUpdated?.Invoke(message);
                });

                _connection.On<double, double>("ReceiveLocationUpdate", (lat, lng) =>
                {
                    OnLocationUpdated?.Invoke(lat, lng);
                });

                await _connection.StartAsync();
            }

            await _connection.InvokeAsync("JoinJobRoom", jobId);
        }

        public async Task StopConnectionAsync(string jobId)
        {
            if (_connection != null)
            {
                await _connection.InvokeAsync("LeaveJobRoom", jobId);
                await _connection.StopAsync();
            }
        }
    }
}
