using Dispatch.API.Hubs;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] DriverStatus status)
        {
            status.Id = Guid.NewGuid();
            status.Timestamp = DateTime.UtcNow;
            _context.DriverStatuses.Add(status);
            await _context.SaveChangesAsync(); 
            
            await _hubContext.Clients.Group(dto.JobId).SendAsync("ReceiveJobUpdate", dto.JobId);

            return Ok(status);
        }
    }
}
