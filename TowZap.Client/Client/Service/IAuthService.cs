using System.Net.Http;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Dispatch.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace TowZap.Client.Client.Service
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequestDTO loginModel);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
        Task<string> GetUserRoleAsync();
        Task<string> GetUserNameAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
