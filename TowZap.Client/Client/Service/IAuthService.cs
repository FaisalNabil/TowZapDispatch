using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;

namespace TowZap.Client.Client.Service
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task LogoutAsync(); 
        Task<bool> RegisterGuestAsync(GuestRegistrationDTO model);

        Task<ProfileDTO> GetProfileAsync(); // Get current user profile
        Task<bool> UpdateProfileAsync(ProfileUpdateDTO model); // Update user info
        Task<bool> ChangePasswordAsync(ChangePasswordDTO model); // Change password

    }
}
