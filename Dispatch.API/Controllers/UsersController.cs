using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;
using Dispatch.Domain.Constants;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Dispatch.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dispatch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = UserRoles.CompanyAdministrator)]
        [HttpGet("company-users")]
        public async Task<IActionResult> GetUsersByCompany()
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdStr)) return Unauthorized();

            var companyId = Guid.Parse(companyIdStr);
            var users = await _userService.GetUsersUnderCompanyAsync(companyId);
            return Ok(users);
        }
        [HttpGet("company-drivers")]
        [Authorize(Roles = UserRoles.Dispatcher)]
        public async Task<IActionResult> GetDriversForCompany()
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdStr, out var companyId))
                return Unauthorized("Invalid company.");

            var drivers = await _userService.GetUsersByRoleAsync("Driver", companyId);
            var result = drivers.Select(u => new
            {
                u.Id,
                FullName = u.FirstName + " " + u.LastName
            });

            return Ok(result);
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
        [HttpPost("admin-create-user")]
        [Authorize(Roles = UserRoles.CompanyAdministrator)]
        public async Task<IActionResult> CreateUserByAdmin([FromBody] AdminCreateUserDTO dto)
        {
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdStr, out var companyId))
                return Unauthorized("Company not found in token.");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                CompanyId = companyId,
                EmailConfirmed = true
            };

            var result = await _userService.RegisterAsync(user, dto.Password, dto.Role, companyId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully.");
        }

        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }


    }
}
