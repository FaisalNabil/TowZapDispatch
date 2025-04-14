using Dispatch.API.Hubs;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Constants;
using Dispatch.Domain.Entities;
using Dispatch.Domain.Enums;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dispatch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<JobUpdateHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobStatusController(
            ApplicationDbContext context,
            IHubContext<JobUpdateHub> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("update-location")]
        [Authorize(Roles = $"{UserRoles.CompanyAdministrator},{UserRoles.Dispatcher},{UserRoles.Driver}")]
        public async Task<IActionResult> UpdateLocation([FromBody] DriverLocationUpdateDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var status = new JobStatusHistory
            {
                Id = Guid.NewGuid(),
                UpdatedByUserId = userId,
                JobRequestId = dto.JobRequestId,
                Status = dto.Status,
                Timestamp = DateTime.UtcNow
            };

            _context.JobStatusHistoies.Add(status);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(dto.JobRequestId.ToString())
                .SendAsync("ReceiveLocationUpdate", dto.Latitude, dto.Longitude);

            return Ok();
        }

        [HttpGet("{jobId}/status-history")]
        [Authorize(Roles = $"{UserRoles.CompanyAdministrator},{UserRoles.Dispatcher},{UserRoles.Driver}")]
        public async Task<ActionResult<List<DriverStatusHistoryItemDTO>>> GetStatusHistory(Guid jobId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return NotFound();

            var history = await _context.JobStatusHistoies
                .Where(d => d.JobRequestId == jobId)
                .OrderByDescending(d => d.Timestamp)
                .Select(d => new DriverStatusHistoryItemDTO
                {
                    Status = (JobStatus)d.Status,
                    Timestamp = d.Timestamp,
                    Note = d.Note,
                    DriverName = _context.Users
                        .Where(u => u.Id == d.UpdatedByUserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}
