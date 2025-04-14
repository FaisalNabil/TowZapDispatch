using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Admin;
using Dispatch.Domain.Constants;
using Dispatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Dispatch.Domain.Entities; // if not already


namespace Dispatch.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AdminDashboardSummaryDTO> GetAdminDashboardSummaryAsync(Guid companyId)
        {
            // Get users in company, including their roles
            var users = await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var jobs = await _context.JobRequests
                .Where(j => j.CompanyId == companyId)
                .Include(j => j.AssignedDriver)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var summary = new AdminDashboardSummaryDTO
            {
                TotalUsers = users.Count,
                DispatcherCount = users.Count(u => u.UserRoles.Any(r => r.Role.Name == UserRoles.Dispatcher)),
                DriverCount = users.Count(u => u.UserRoles.Any(r => r.Role.Name == UserRoles.Driver)),
                GuestCount = users.Count(u => u.UserRoles.Any(r => r.Role.Name == UserRoles.GuestUser)),

                TotalJobs = jobs.Count,
                JobStatusCounts = jobs
                    .GroupBy(j => j.Status.ToString())
                    .Select(g => new JobStatusCountDTO
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),

                RecentJobs = jobs.Take(5).Select(j => new RecentJobDTO
                {
                    Id = j.Id,
                    PlateNumber = j.PlateNumber,
                    Status = j.Status.ToString(),
                    DriverName = j.AssignedDriver != null
                        ? $"{j.AssignedDriver.FirstName} {j.AssignedDriver.LastName}"
                        : "Unassigned",
                    CreatedAt = j.CreatedAt
                }).ToList()
            };

            return summary;
        }
    }
}
