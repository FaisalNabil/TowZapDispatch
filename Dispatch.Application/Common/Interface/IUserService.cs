using Dispatch.Application.DTOs.Auth;
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
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role, Guid? companyId = null);
        Task<IdentityResult> ApproveDriverAsync(string userId);
    }
}
