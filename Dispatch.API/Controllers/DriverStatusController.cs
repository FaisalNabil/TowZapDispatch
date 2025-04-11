using Dispatch.API.Hubs;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Entities;
using Dispatch.Domain.Enums;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Dispatch.API.Controllers
{
    [Authorize(Roles = "Driver")]
    [Route("api/[controller]/status")]
    [ApiController]
    public class DriverStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<JobUpdateHub> _hubContext;

        public DriverStatusController(ApplicationDbContext context,
            IHubContext<JobUpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] DriverLocationUpdateDTO dto)
        {
            var driverUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (driverUserId == null)
                return Unauthorized();

            var status = new DriverStatus
            {
                Id = Guid.NewGuid(),
                DriverUserId = driverUserId,
                JobRequestId = dto.JobRequestId,
                Status = dto.Status,
                Timestamp = DateTime.UtcNow
            };

            _context.DriverStatuses.Add(status);

            await _context.SaveChangesAsync();
            var job = await _context.JobRequests.FindAsync(dto.JobRequestId);
            if (job != null)
            {
                if (Enum.TryParse<JobStatus>(dto.Status, out var parsedStatus))
                {
                    job.Status = parsedStatus;
                }
            }

            await _context.SaveChangesAsync();

            // Broadcast location to group using SignalR
            await _hubContext.Clients.Group(dto.JobRequestId.ToString())
                .SendAsync("ReceiveLocationUpdate", dto.Latitude, dto.Longitude);

            return Ok();
        }


    }
}
