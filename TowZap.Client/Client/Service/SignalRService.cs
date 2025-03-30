using Microsoft.AspNetCore.SignalR.Client;

namespace TowZap.Client.Client.Service
{
    public class SignalRService
    {
        private HubConnection _hubConnection;

        public event Action<string> OnJobUpdated;

        public async Task StartAsync(string baseUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/jobhub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("JobUpdated", (message) =>
            {
                OnJobUpdated?.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_hubConnection != null)
                await _hubConnection.StopAsync();
        }
    }
}
