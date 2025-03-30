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
        [Authorize(Roles = "Dispatcher")]
        public IActionResult GetByDispatcher()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
            var jobs = _context.JobRequests.Where(j => j.CreatedByUserId == userId).ToList();
            return Ok(jobs);
        }

        [HttpGet("driver")]
        [Authorize(Roles = "Driver")]
        public IActionResult GetByDriver()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
            var jobIds = _context.DriverStatuses.Where(s => s.DriverUserId == userId).Select(s => s.JobRequestId).Distinct();
            var jobs = _context.JobRequests.Where(j => jobIds.Contains(j.Id)).ToList();
            return Ok(jobs);
        }

        [HttpGet("guest")]
        [Authorize(Roles = "GuestUser")]
        public IActionResult GetForGuest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = _context.JobRequests.Where(j => j.CreatedByUserId == userId).ToList();
            return Ok(jobs);
        }


        [HttpPost]
        [Authorize(Roles = "Dispatcher,Driver")]
        public async Task<IActionResult> Create([FromForm] JobRequest request, IFormFile? photo)
        {
            if (photo != null)
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                request.PhotoPath = fileName;
            }

            request.Id = Guid.NewGuid();
            request.TowDate = request.TowDate == default ? DateTime.UtcNow : request.TowDate;
            request.CreatedByUserId = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;

            if (!request.IsImpoundTriggered && request.AddressTo?.ToLower().Contains("vsf") == true)
                request.IsImpoundTriggered = true;

            _context.JobRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(request);
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
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> AssignJob(Guid jobId, [FromQuery] string driverUserId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return NotFound("Job not found.");

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