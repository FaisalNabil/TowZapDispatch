using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Dispatch.Application.DTOs.Registration;

namespace TowZap.Client.Client.Service
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task LogoutAsync(); 
        Task<bool> RegisterGuestAsync(GuestRegistrationDTO model);

    }
}
