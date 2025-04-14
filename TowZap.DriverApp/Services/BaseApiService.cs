using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TowZap.DriverApp.Services
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;

        private string _cachedToken;

        public BaseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task AddAuthHeaderAsync()
        {
            if (string.IsNullOrEmpty(_cachedToken))
            {
                _cachedToken = await SecureStorage.GetAsync("auth_token");
            }

            if (!string.IsNullOrEmpty(_cachedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
            }
        }


        protected async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"GET {endpoint} => Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GET failed: {json}");
                    return default;
                }

                return JsonSerializer.Deserialize(json, AppJsonContext.Default.GetTypeInfo(typeof(T))) as T;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET {endpoint} threw an exception: {ex.Message}");
                return default;
            }
        }

        protected async Task<T?> PostAsync<TRequest, T>(string endpoint, TRequest body)
            where T : class
        {
            try
            {
                await AddAuthHeaderAsync();

                var requestJson = JsonSerializer.Serialize(body, AppJsonContext.Default.GetTypeInfo(typeof(TRequest)));
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"POST {endpoint} => Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"POST failed: {responseBody}");
                    return default;
                }

                return JsonSerializer.Deserialize(responseBody, AppJsonContext.Default.GetTypeInfo(typeof(T))) as T;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST {endpoint} threw an exception: {ex.Message}");
                return default;
            }
        }

        protected async Task<bool> PutAsync<T>(string endpoint, T body) 
        {
            try
            {
                await AddAuthHeaderAsync();

                var json = JsonSerializer.Serialize(body); 
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"PUT {endpoint} => Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"PUT failed: {result}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT {endpoint} threw an exception: {ex.Message}");
                return false;
            }
        }

    }
}
