using Dispatch.Domain.Constants;
using Dispatch.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed roles
            var roles = new[]
            {
                UserRoles.Administrator,
                UserRoles.CompanyAdministrator,
                UserRoles.Dispatcher,
                UserRoles.Driver,
                UserRoles.GuestUser
            };
            foreach (var roleName in roles)
            {
                var exists = await context.Roles.AnyAsync(r => r.Name == roleName);
                if (!exists)
                {
                    context.Roles.Add(new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        Discriminator = "UserRole"
                    });
                }
            }

            // Seed admin user
            var adminEmail = "admin@tousif.com";
            var adminExists = await context.Users.AnyAsync(u => u.Email == adminEmail);
            if (!adminExists)
            {
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Admin@123"),
                    CompanyId = Guid.Empty,

                    // Required Identity fields
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var adminRoleId = await context.Roles
                    .Where(r => r.Name == "Administrator")
                    .Select(r => r.Id)
                    .FirstAsync();

                context.Users.Add(adminUser);
                context.UserRoles.Add(new ApplicationUserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRoleId
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
