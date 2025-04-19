using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs;
using Dispatch.Application.DTOs.Auth;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dispatch.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }


        public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<LoginResponseDTO>.Fail("Email or password must not be empty.");

            var user = await _context.Users
                                 .Where(x => x.Email == request.Email)
                                 .SingleOrDefaultAsync();

            if (user == null)
                return ApiResponse<LoginResponseDTO>.Fail("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return ApiResponse<LoginResponseDTO>.Fail("Invalid email or password.");

            var role = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)  // Assuming navigation property is set
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(role))
                return ApiResponse<LoginResponseDTO>.Fail("User role not found.");

            string companyName = await _context.Companies
                .Where(c => c.Id == user.CompanyId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync() ?? "Unknown Company";

            user.CompanyName = companyName;

            var token = _jwtTokenService.GenerateToken(user, new List<string> { role });

            var response = new LoginResponseDTO
            {
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = role,
                CompanyName = companyName,
                UserId = user.Id
            };

            return ApiResponse<LoginResponseDTO>.Success(response);
        }

    }
}
