using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;
using Dispatch.Application.DTOs;

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

            // Read and parse the full ApiResponse<LoginResponseDTO>
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();

            if (apiResponse == null || !apiResponse.IsSuccess)
                throw new Exception(apiResponse?.Message ?? "Login failed");

            var loginResponse = apiResponse.Data;

            if (loginResponse == null)
                throw new Exception("Invalid response from server");

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
        public async Task<ProfileDTO> GetProfileAsync()
        {
            var response = await _http.GetAsync("api/auth/profile");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch profile");

            return await response.Content.ReadFromJsonAsync<ProfileDTO>();
        }

        public async Task<bool> UpdateProfileAsync(ProfileUpdateDTO model)
        {
            var response = await _http.PutAsJsonAsync("api/auth/profile", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO model)
        {
            var response = await _http.PutAsJsonAsync("api/auth/change-password", model);
            return response.IsSuccessStatusCode;
        }

    }
}
