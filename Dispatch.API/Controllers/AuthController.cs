using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Auth;
using Dispatch.Application.DTOs.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dispatch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _userService.LoginAsync(request);
            if (response == null)
                return Unauthorized("Invalid email or password");

            return Ok(response);
        }

        [HttpPost("request-driver")]
        public async Task<IActionResult> RequestDriver([FromBody] DriverRegistrationRequestDTO dto)
        {
            var result = await _userService.RegisterAsync(dto.ToUser(), dto.Password, "PendingDriver");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Driver registration request submitted.");
        }

        [HttpPost("register-dispatcher")]
        public async Task<IActionResult> RegisterDispatcher([FromBody] DispatcherRegistrationDTO dto)
        {
            var result = await _userService.RegisterAsync(dto.ToUser(), dto.Password, "Dispatcher");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Dispatcher registered successfully.");
        }

        [HttpPost("register-guest")]
        public async Task<IActionResult> RegisterGuest([FromBody] GuestRegistrationDTO dto)
        {
            var result = await _userService.RegisterAsync(dto.ToUser(), dto.Password, "GuestUser");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Guest user registered.");
        }

        [Authorize(Roles = "Administrator,Dispatcher")]
        [HttpPost("approve-driver/{userId}")]
        public async Task<IActionResult> ApproveDriver(string userId)
        {
            var result = await _userService.ApproveDriverAsync(userId);
            if (!result.Succeeded)
                return BadRequest("User is not a pending driver.");

            return Ok("Driver approved.");
        }
    }
}
