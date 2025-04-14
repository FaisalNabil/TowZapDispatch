using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TowZap.DriverApp.Services
{
    public class SignalRClientService
    {
        private HubConnection _connection;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task InitializeAsync(string hubUrl, string token)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async (error) =>
            {
                Console.WriteLine($"SignalR closed: {error?.Message}");
                await Task.Delay(2000);
                await _connection.StartAsync();
            };

            await _connection.StartAsync();
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            _connection.On(methodName, handler);
        }

        public void On<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            _connection.On(methodName, handler);
        }

        public async Task StopAsync()
        {
            if (_connection != null)
                await _connection.StopAsync();
        }

        public async Task SendAsync(string method, params object[] args)
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
                await _connection.SendAsync(method, args);
        }
    }
}
