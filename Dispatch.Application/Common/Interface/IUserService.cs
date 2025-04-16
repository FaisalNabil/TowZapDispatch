using Dispatch.Application.DTOs.Auth;
using Dispatch.Application.DTOs.User;
using Dispatch.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.Common.Interface
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role, Guid? companyId = null);
        Task<IdentityResult> ApproveDriverAsync(string userId); 
        Task<List<UserSummaryDTO>> GetUsersUnderCompanyAsync(Guid companyId); 
        Task<ApplicationUser?> GetUserByIdAsync(string userId);

        Task<IdentityResult> PromoteUserAsync(string userId, string newRole); 
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string role, Guid companyId);
        Task<ProfileDTO> GetProfileAsync(string userId);
        Task<IdentityResult> UpdateProfileAsync(string userId, ProfileUpdateDTO model);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    }
}
