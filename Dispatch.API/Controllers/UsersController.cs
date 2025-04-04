using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.User;
using Dispatch.Domain.Constants;
using Dispatch.Infrastructure.Persistence;
using Dispatch.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dispatch.API.Controllers
{
    [Authorize(Roles = UserRoles.CompanyAdministrator)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("company-users")]
        public async Task<IActionResult> GetUsersByCompany()
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdStr)) return Unauthorized();

            var companyId = Guid.Parse(companyIdStr);
            var users = await _userService.GetUsersUnderCompanyAsync(companyId);
            return Ok(users);
        }
        [HttpPost("promote-to-dispatcher/{userId}")]
        [Authorize(Roles = UserRoles.CompanyAdministrator)]
        public async Task<IActionResult> PromoteToDispatcher(string userId)
        {
            var companyId = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyId))
                return Unauthorized("Company context missing.");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (user.CompanyId != Guid.Parse(companyId))
                return Forbid("You can only promote users from your own company.");

            var result = await _userService.PromoteUserAsync(userId, UserRoles.Dispatcher);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User promoted to Dispatcher.");
        }

    }
}
