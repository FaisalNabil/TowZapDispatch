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
using Dispatch.Application.DTOs.User;
using static System.Net.WebRequestMethods;

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
            var user = await _db.Users
                                 .Where(x => x.Email == request.Email)
                                 .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var role = await _db.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)  // Assuming navigation property is set
                .FirstOrDefaultAsync();

            var token = _jwtTokenService.GenerateToken(user, new List<string> { role });

            return new LoginResponseDTO
            {
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = role
            };
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role, Guid? companyId = null)
        {
            if (string.IsNullOrWhiteSpace(user.Id))
                user.Id = Guid.NewGuid().ToString();

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
        public async Task<List<UserSummaryDTO>> GetUsersUnderCompanyAsync(Guid companyId)
        {
            var users = await _db.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => new UserSummaryDTO
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = (from ur in _db.UserRoles
                            join r in _db.Roles on ur.RoleId equals r.Id
                            where ur.UserId == u.Id
                            select r.Name).FirstOrDefault(),
                    IsActive = !u.IsDisabled
                })
                .ToListAsync();

            return users;
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IdentityResult> PromoteUserAsync(string userId, string newRole)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            // Remove existing role(s)
            var userRoles = _db.UserRoles.Where(ur => ur.UserId == userId);
            _db.UserRoles.RemoveRange(userRoles);

            // Add new role
            var newRoleId = await _db.Roles.Where(r => r.Name == newRole).Select(r => r.Id).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(newRoleId))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{newRole}' does not exist." });

            _db.UserRoles.Add(new ApplicationUserRole
            {
                UserId = userId,
                RoleId = newRoleId
            });

            await _db.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string role, Guid companyId)
        {
            var users = await _db.UserRoles
                .Where(ur => ur.Role.Name == role) // Assuming navigation property
                .Select(ur => ur.User)
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();

            return users;
        }
    }
}
