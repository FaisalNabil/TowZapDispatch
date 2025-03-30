using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Auth;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;

        public UserService(ApplicationDbContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var roles = await _db.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .ToListAsync();

            var token = _jwtTokenService.GenerateToken(user, roles);

            return new LoginResponseDTO
            {
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = roles.FirstOrDefault() ?? ""
            };
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role, Guid? companyId = null)
        {
            // Hash the password
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Save user
            _db.Users.Add(user);

            var roleId = await _db.Roles.Where(r => r.Name == role).Select(r => r.Id).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(roleId))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{role}' does not exist." });

            _db.UserRoles.Add(new ApplicationUserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });

            try
            {
                await _db.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> ApproveDriverAsync(string userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var pendingRoleId = await _db.Roles.Where(r => r.Name == "PendingDriver").Select(r => r.Id).FirstOrDefaultAsync();
            var driverRoleId = await _db.Roles.Where(r => r.Name == "Driver").Select(r => r.Id).FirstOrDefaultAsync();

            var userRole = await _db.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId && r.RoleId == pendingRoleId);
            if (userRole == null)
                return IdentityResult.Failed(new IdentityError { Description = "User is not a pending driver." });

            _db.UserRoles.Remove(userRole);
            _db.UserRoles.Add(new ApplicationUserRole
            {
                UserId = userId,
                RoleId = driverRoleId
            });

            await _db.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }
}
