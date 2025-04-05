using Dispatch.Domain.Constants;
using Dispatch.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dispatch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MetaController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("expiry")]
        public IActionResult GetExpiryDate()
        {
            var expiryStr = _config["AppSecurity:ExpiryDate"];
            return Ok(new { expiry = expiryStr });
        }

        [HttpGet("tow-reasons")]
        public IActionResult GetTowReasons()
        {
            var reasons = Enum.GetNames(typeof(TowReason)).Select(name => new { Value = name, Label = name }).ToList();
            return Ok(reasons);
        }

        [HttpGet("job-statuses")]
        public IActionResult GetJobStatuses()
        {
            var statuses = Enum.GetValues(typeof(JobStatus))
                .Cast<JobStatus>()
                .Select(e => new { value = e.ToString(), label = e.ToString() })
                .ToList();

            return Ok(statuses);
        }
        [HttpGet("user-roles")]
        public IActionResult GetAssignableRoles()
        {
            var roles = new[]
            {
                new { value = UserRoles.Dispatcher, label = "Dispatcher" },
                new { value = UserRoles.Driver, label = "Driver" },
                new { value = UserRoles.GuestUser, label = "Guest User" }
            };

            return Ok(roles);
        }


    }
}
