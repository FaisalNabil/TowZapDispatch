using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Dispatch.Application.DTOs.Registration;

namespace TowZap.Client.Client.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Invalid email or password");

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

            await _localStorage.SetItemAsync("authToken", loginResponse.Token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginResponse.Token);

            return loginResponse;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
        public async Task<bool> RegisterGuestAsync(GuestRegistrationDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register-guest", dto);
            return response.IsSuccessStatusCode;
        }

    }
}
