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
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }


        public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role, Guid? companyId = null)
        {
            if (string.IsNullOrWhiteSpace(user.Id))
                user.Id = Guid.NewGuid().ToString();

            // Hash the password
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Save user
            _context.Users.Add(user);

            var roleId = await _context.Roles.Where(r => r.Name == role).Select(r => r.Id).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(roleId))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{role}' does not exist." });

            _context.UserRoles.Add(new ApplicationUserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });

            try
            {
                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> ApproveDriverAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var pendingRoleId = await _context.Roles.Where(r => r.Name == "PendingDriver").Select(r => r.Id).FirstOrDefaultAsync();
            var driverRoleId = await _context.Roles.Where(r => r.Name == "Driver").Select(r => r.Id).FirstOrDefaultAsync();

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId && r.RoleId == pendingRoleId);
            if (userRole == null)
                return IdentityResult.Failed(new IdentityError { Description = "User is not a pending driver." });

            _context.UserRoles.Remove(userRole);
            _context.UserRoles.Add(new ApplicationUserRole
            {
                UserId = userId,
                RoleId = driverRoleId
            });

            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public async Task<List<UserSummaryDTO>> GetUsersUnderCompanyAsync(Guid companyId)
        {
            var users = await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => new UserSummaryDTO
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == u.Id
                            select r.Name).FirstOrDefault(),
                    IsActive = !u.IsDisabled
                })
                .ToListAsync();

            return users;
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IdentityResult> PromoteUserAsync(string userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            // Remove existing role(s)
            var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
            _context.UserRoles.RemoveRange(userRoles);

            // Add new role
            var newRoleId = await _context.Roles.Where(r => r.Name == newRole).Select(r => r.Id).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(newRoleId))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{newRole}' does not exist." });

            _context.UserRoles.Add(new ApplicationUserRole
            {
                UserId = userId,
                RoleId = newRoleId
            });

            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string role, Guid companyId)
        {
            var users = await _context.UserRoles
                .Where(ur => ur.Role.Name == role) // Assuming navigation property
                .Select(ur => ur.User)
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();

            return users;
        }
        public async Task<ProfileDTO> GetProfileAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user == null ? null : new ProfileDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };
        }

        public async Task<IdentityResult> UpdateProfileAsync(string userId, ProfileUpdateDTO model)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return IdentityResult.Failed();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Phone;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return IdentityResult.Failed();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
                return IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect." });

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

    }
}
