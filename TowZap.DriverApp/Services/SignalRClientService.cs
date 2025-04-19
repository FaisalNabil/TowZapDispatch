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
        private readonly HashSet<string> _registeredHandlers = new();
        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task InitializeAsync(string hubUrl, string token, string? jobId = null)
        {
            var fullUrl = jobId != null ? $"{hubUrl}?jobId={jobId}" : hubUrl;

            _connection = new HubConnectionBuilder()
                .WithUrl(fullUrl, options =>
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

            _connection.Reconnected += connectionId =>
            {
                Console.WriteLine($"SignalR reconnected: {connectionId}");
                return Task.CompletedTask;
            };

            _connection.Reconnecting += error =>
            {
                Console.WriteLine($"SignalR reconnecting: {error?.Message}");
                return Task.CompletedTask;
            };

            Console.WriteLine($"Connecting to SignalR at: {fullUrl}");

            await _connection.StartAsync();

            Console.WriteLine($"SignalR connected: {_connection.State}");
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection), "HubConnection is not initialized. Call InitializeAsync first.");

            if (_registeredHandlers.Contains(methodName)) return;

            _connection.On(methodName, handler);
            _registeredHandlers.Add(methodName);
        }

        public void On<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection), "HubConnection is not initialized. Call InitializeAsync first.");

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
