using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dispatch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LettersController : ControllerBase
    {
        private readonly NotificationLetterService _letterService;

        public LettersController(NotificationLetterService letterService)
        {
            _letterService = letterService;
        }

        [HttpGet("generate/{jobId}")]
        public async Task<IActionResult> Generate(Guid jobId)
        {
            var path = await _letterService.Generate1stLetterAsync(jobId);
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf", Path.GetFileName(path));
        }
    }
}
