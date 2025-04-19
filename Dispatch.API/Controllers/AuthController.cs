using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Auth;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;
using Dispatch.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dispatch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _authService.LoginAsync(request);

            if (!response.IsSuccess)
                return Unauthorized(response); 

            return Ok(response);
        }

        [Authorize(Roles = UserRoles.CompanyAdministrator + "," + UserRoles.Dispatcher)]
        [HttpPost("approve-driver/{userId}")]
        public async Task<IActionResult> ApproveDriver(string userId)
        {
            var result = await _userService.ApproveDriverAsync(userId);
            if (!result.Succeeded)
                return BadRequest("User is not a pending driver.");

            return Ok("Driver approved.");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _userService.GetProfileAsync(userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.UpdateProfileAsync(userId, model);

            if (result.Succeeded) return Ok("Profile updated");
            return BadRequest(result.Errors);
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded) return Ok("Password changed");
            return BadRequest(result.Errors);
        }

    }
}
