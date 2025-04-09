using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using Dispatch.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Dispatch.Domain.Constants;

namespace Dispatch.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequestsController : ControllerBase
    {
        private readonly IJobRequestService _jobService;
        private readonly IHubContext<JobUpdateHub> _hubContext;

        public JobRequestsController(IJobRequestService jobService, IHubContext<JobUpdateHub> hubContext)
        {
            _jobService = jobService;
            _hubContext = hubContext;
        }

        [HttpGet("dispatcher")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> GetByDispatcher()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await _jobService.GetJobsForDispatcherAsync(userId);
            return Ok(jobs);
        }

        [HttpGet("driver")]
        [Authorize(Roles = UserRoles.Driver)]
        public async Task<IActionResult> GetByDriver()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await _jobService.GetJobsForDriverAsync(userId);
            return Ok(jobs);
        }

        [HttpGet("company")]
        [Authorize(Roles = UserRoles.CompanyAdministrator)]
        public async Task<IActionResult> GetForCompany()
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdStr)) return Unauthorized("Company not found.");
            var companyId = Guid.Parse(companyIdStr);

            var jobs = await _jobService.GetJobsForCompanyAsync(companyId);
            return Ok(jobs);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{UserRoles.Dispatcher},{UserRoles.CompanyAdministrator}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        [HttpPost("create")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> Create([FromBody] CreateJobRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // This will return all validation errors
            }
            var dispatcherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var companyId = User.FindFirst("CompanyId")?.Value;

            if (dispatcherId == null || string.IsNullOrWhiteSpace(companyId))
                return Unauthorized("Missing dispatcher or company.");

            var jobId = await _jobService.CreateJobAsync(dto, dispatcherId, Guid.Parse(companyId));

            await _hubContext.Clients.Group(companyId).SendAsync("JobCreated", jobId);
            return Ok(new { jobId, Message = "Job created and driver assigned." });
        }

        [HttpPut("{jobId}/status")]
        [Authorize(Roles = $"{UserRoles.Dispatcher},{UserRoles.Driver}")]
        public async Task<IActionResult> UpdateStatus(Guid jobId, [FromBody] JobStatus newStatus)
        {
            var updated = await _jobService.UpdateJobStatusAsync(jobId, newStatus);
            if (!updated) return NotFound("Job not found.");

            var job = await _jobService.GetJobByIdAsync(jobId);
            await _hubContext.Clients.Group(job.CompanyId.ToString()).SendAsync("JobStatusUpdated", jobId, newStatus.ToString());

            return Ok("Status updated.");
        }

        [HttpPost("{jobId}/assign")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> AssignJob(Guid jobId, [FromQuery] string driverUserId)
        {
            var result = await _jobService.AssignDriverAsync(jobId, driverUserId);
            if (!result) return NotFound("Job not found or failed to assign.");

            await _hubContext.Clients.Group(jobId.ToString()).SendAsync("ReceiveJobUpdate", jobId);
            return Ok("Driver assigned.");
        }

        [HttpDelete("{jobId}")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> DeleteJob(Guid jobId)
        {
            var result = await _jobService.DeleteJobAsync(jobId);
            if (!result) return NotFound("Job not found.");
            return Ok("Job deleted.");
        }
    }
}