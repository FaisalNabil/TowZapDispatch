using Dispatch.Application.Common.Interface;
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


        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _context.Users
                                 .Where(x => x.Email == request.Email)
                                 .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var role = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)  // Assuming navigation property is set
                .FirstOrDefaultAsync();

            string companyName = await _context.Companies
        .Where(c => c.Id == user.CompanyId)
        .Select(c => c.Name)
        .FirstOrDefaultAsync() ?? "Unknown Company";

            user.CompanyName = companyName;

            var token = _jwtTokenService.GenerateToken(user, new List<string> { role });

            return new LoginResponseDTO
            {
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = role,
                CompanyName = companyName
            };
        }
    }
}
