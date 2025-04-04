using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using System.Security.Claims;
using Dispatch.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using System.Data;
using System.Dynamic;
using Dispatch.Domain.Constants;

namespace Dispatch.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<JobUpdateHub> _hubContext;

        public JobRequestsController(ApplicationDbContext context, 
            IWebHostEnvironment environment, 
            IHubContext<JobUpdateHub> hubContext)
        {
            _context = context;
            _environment = environment;
            _hubContext = hubContext;
        }

        [HttpGet("dispatcher")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public IActionResult GetByDispatcher()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = _context.JobRequests
                .Where(j => j.AssignedDriverId == userId)
                .ToList();
            return Ok(jobs);
        }

        [HttpGet("driver")]
        [Authorize(Roles = UserRoles.Driver)]
        public IActionResult GetByDriver()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = _context.JobRequests
                .Where(j => j.AssignedDriverId == userId)
                .ToList();
            return Ok(jobs);
        }
        [HttpGet("company")]
        [Authorize(Roles = UserRoles.CompanyAdministrator)]
        public IActionResult GetForCompany()
        {
            var companyId = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("Company not found.");
            var parsed = Guid.Parse(companyId);
            var jobs = _context.JobRequests
                .Where(j => j.CompanyId == parsed)
                .ToList();
            return Ok(jobs);
        }

        [HttpDelete("{jobId}")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return NotFound("Job not found.");

            _context.JobRequests.Remove(job);
            await _context.SaveChangesAsync();
            return Ok("Job deleted successfully.");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = UserRoles.Dispatcher + "," + UserRoles.CompanyAdministrator)]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _context.JobRequests.FindAsync(id);
            if (job == null) return NotFound();

            var dto = new JobResponseDTO
            {
                Id = job.Id,
                CallerName = job.CallerName,
                CallerPhone = job.CallerPhone,
                Make = job.Make,
                Model = job.Model,
                PlateNumber = job.PlateNumber,
                Reason = job.Reason,
                FromLocation = job.FromLocation,
                ToLocation = job.ToLocation,
                AssignedDriverId = job.AssignedDriverId,
                AssignedTowTruck = job.AssignedTowTruck,
                Status = job.Status,
                CreatedAt = job.CreatedAt
            };

            return Ok(dto);
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> Create([FromBody] CreateJobRequestDTO dto)
        {
            try
            {
                var dispatcherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var companyId = User.FindFirst("CompanyId")?.Value;

                if (dispatcherId == null || string.IsNullOrWhiteSpace(companyId))
                    return Unauthorized("Missing user or company context.");

                var job = new JobRequest
                {
                    AccountName = dto.AccountName,
                    CallerName = dto.CallerName,
                    CallerPhone = dto.CallerPhone,

                    VIN = dto.VIN,
                    Year = dto.Year,
                    Make = dto.Make,
                    Model = dto.Model,
                    Color = dto.Color,
                    PlateNumber = dto.PlateNumber,
                    StatePlate = dto.StatePlate,
                    Keys = dto.Keys,
                    UnitNumber = dto.UnitNumber,
                    Odometer = dto.Odometer,

                    Reason = dto.Reason,
                    FromLocation = dto.FromLocation,
                    ToLocation = dto.ToLocation,

                    AssignedDriverId = dto.AssignedDriverId,
                    AssignedTowTruck = dto.AssignedTowTruck,
                    Status = JobStatus.Assigned, // Dispatcher assigns on creation

                    CreatedAt = DateTime.UtcNow,
                    CompanyId = Guid.Parse(companyId)
                };

                _context.JobRequests.Add(job);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Group(job.CompanyId.ToString()).SendAsync("JobCreated", job.Id);

                return Ok(new { job.Id, Message = "Job created and driver assigned." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create job: {ex.Message}");
            }
        }


        [HttpGet("photo/{filename}")]
        public IActionResult ViewPhoto(string filename)
        {
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsDir, filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Photo not found.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "image/jpeg");
        }

        [HttpPost("{jobId}/assign")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> AssignJob(Guid jobId, [FromQuery] string driverUserId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return NotFound("Job not found.");

            job.AssignedDriverId = driverUserId;
            job.Status = JobStatus.Assigned;
            await _context.SaveChangesAsync();

            var status = new DriverStatus
            {
                Id = Guid.NewGuid(),
                JobRequestId = jobId,
                DriverUserId = driverUserId,
                Timestamp = DateTime.UtcNow,
                Status = "Assigned"
            };

            _context.DriverStatuses.Add(status);
            await _context.SaveChangesAsync(); 
            
            await _hubContext.Clients.Group(jobId.ToString()).SendAsync("ReceiveJobUpdate", jobId);

            return Ok("Driver assigned successfully.");
        }
        [HttpPut("{jobId}/status")]
        [Authorize(Roles = $"{UserRoles.Dispatcher},{UserRoles.Driver}")]
        public async Task<IActionResult> UpdateStatus(int jobId, [FromBody] JobStatus newStatus)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return NotFound("Job not found.");

            job.Status = newStatus;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(job.CompanyId.ToString())
                .SendAsync("JobStatusUpdated", jobId, newStatus.ToString());

            return Ok("Status updated.");
        }


        //[HttpPost("{jobId}/generate-notification-template")]
        //[Authorize(Roles = "Dispatcher")]
        //public async Task<IActionResult> GenerateNotificationLetterWithTemplate(Guid jobId, [FromQuery] string type = "1st")
        //{
        //    var job = await _context.JobRequests.Include(j => j.CreatedByUser)
        //                .FirstOrDefaultAsync(j => j.Id == jobId);

        //    if (job == null) return NotFound("Job not found.");

        //    var templatePath = Path.Combine(_environment.WebRootPath, "templates", "NotificationTemplate.pdf");
        //    var lettersDir = Path.Combine(_environment.WebRootPath, "letters");
        //    if (!Directory.Exists(lettersDir)) Directory.CreateDirectory(lettersDir);

        //    var outputPath = Path.Combine(lettersDir, $"{job.Id}_{type}.pdf");

        //    using (var reader = new PdfReader(templatePath))
        //    using (var writer = new PdfWriter(outputPath))
        //    using (var pdfDoc = new PdfDocument(reader, writer))
        //    {
        //        var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
        //        var fields = form.GetFormFields();

        //        fields["LicensePlate"].SetValue(job.LicensePlate ?? "");
        //        fields["TowDate"].SetValue(job.TowDate.ToString("yyyy-MM-dd"));
        //        fields["AddressTo"].SetValue(job.AddressTo ?? "");

        //        form.FlattenFields(); // Optional: makes the text uneditable
        //        pdfDoc.Close();
        //    }

        //    var letter = new NotificationLetter
        //    {
        //        Id = Guid.NewGuid(),
        //        JobRequestId = job.Id,
        //        LetterType = type,
        //        GeneratedOn = DateTime.UtcNow,
        //        FilePath = Path.GetFileName(outputPath)
        //    };

        //    _context.NotificationLetters.Add(letter);
        //    await _context.SaveChangesAsync();

        //    return Ok($"Notification letter ({type}) generated with template.");
        //}
    }
}