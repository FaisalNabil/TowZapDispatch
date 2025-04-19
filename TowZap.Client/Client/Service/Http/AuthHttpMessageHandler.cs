using Blazored.LocalStorage;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace TowZap.Client.Client.Service.Http
{
    public class AuthHttpMessageHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigation;
        private readonly ILocalStorageService _localStorage;

        public AuthHttpMessageHandler(NavigationManager navigation, ILocalStorageService localStorage)
        {
            _navigation = navigation;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            var isLoginEndpoint = request.RequestUri?.AbsolutePath.Contains("auth/login") ?? false;

            if (response.StatusCode == HttpStatusCode.Unauthorized && !isLoginEndpoint)
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigation.NavigateTo("/login", true); // force redirect
            }

            return response;
        }
    }
}
