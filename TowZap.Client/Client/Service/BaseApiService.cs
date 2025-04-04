using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace TowZap.Client.Client.Service
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _http;
        protected readonly ILocalStorageService _localStorage;

        public BaseApiService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        protected async Task AddBearerTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
