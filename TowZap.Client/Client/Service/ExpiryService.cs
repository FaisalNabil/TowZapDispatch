using System.Net.Http.Json;

namespace TowZap.Client.Client.Service
{
    public class ExpiryService
    {
        private readonly HttpClient _http;

        public ExpiryService(HttpClient http)
        {
            _http = http;
        }

        public async Task<DateTime?> GetExpiryAsync()
        {
            var result = await _http.GetFromJsonAsync<ExpiryResponse>("api/meta/expiry");
            return DateTime.TryParse(result?.Expiry, out var date) ? date : null;
        }

        private class ExpiryResponse
        {
            public string Expiry { get; set; }
        }
    }
}
