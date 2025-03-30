using Microsoft.AspNetCore.SignalR.Client;

namespace TowZap.Client.Client.Service
{
    public class JobUpdateService
    {
        private HubConnection _connection;

        public event Action<string> OnJobUpdated;

        public async Task StartConnectionAsync(string jobId)
        {
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/hubs/jobUpdates")
                    .WithAutomaticReconnect()
                    .Build();

                _connection.On<string>("ReceiveJobUpdate", (message) =>
                {
                    OnJobUpdated?.Invoke(message);
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
