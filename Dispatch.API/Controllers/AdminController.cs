using Dispatch.API.Hubs;
using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Admin;
using Dispatch.Domain.Constants;
using Dispatch.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Dispatch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.CompanyAdministrator)]
        [HttpGet("summary")]
        public async Task<ActionResult<AdminDashboardSummaryDTO>> GetDashboardSummary()
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdStr)) return Unauthorized("Company not found.");
            var companyId = Guid.Parse(companyIdStr);

            var summary = await _adminService.GetAdminDashboardSummaryAsync(companyId);
            return Ok(summary);
        }
    }
}
