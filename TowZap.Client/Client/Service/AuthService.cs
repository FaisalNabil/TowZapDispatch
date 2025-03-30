using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace TowZap.Client.Client.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        private const string TokenKey = "token";

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<bool> LoginAsync(LoginRequestDTO loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
                return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            if (loginResponse != null)
            {
                await _localStorage.SetItemAsync("token", loginResponse.Token);
                await _localStorage.SetItemAsync("role", loginResponse.Role);
                await _localStorage.SetItemAsync("user", JsonSerializer.Serialize(loginResponse));
                return true;
            }

            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginResponse.Token);
            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("role");
            await _localStorage.RemoveItemAsync("user");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("token");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string> GetUserRoleAsync()
        {
            var token = await GetTokenAsync();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }

        public async Task<string> GetUserNameAsync()
        {
            var token = await GetTokenAsync();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
