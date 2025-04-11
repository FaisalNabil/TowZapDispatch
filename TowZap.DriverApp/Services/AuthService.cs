using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Models;

namespace TowZap.DriverApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        //{
        //    Console.WriteLine("Sending login request:");
        //    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(request));

        //    var response = await _httpClient.PostAsJsonAsync("auth/login", request);

        //    var content = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine($"Response Status: {response.StatusCode}");
        //    Console.WriteLine($"Response Body: {content}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(content);
        //    }

        //    return null;
        //}


        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }

            return null;
        }
    }
}
