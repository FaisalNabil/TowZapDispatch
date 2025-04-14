using Dispatch.Application.DTOs.Admin;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;
using TowZap.Client.Client.DTOs;

namespace TowZap.Client.Client.Service
{
    public interface IClientUserService
    {
        Task<List<UserSummaryDTO>> GetUsersByCompanyAsync();
        Task<bool> PromoteToDispatcherAsync(string userId); 
        Task<bool> CreateUserByAdminAsync(AdminCreateUserDTO dto);
        Task<List<DriverDropdownDTO>> GetDriversInCompanyAsync();
        Task<AdminDashboardSummaryDTO?> GetAdminDashboardSummaryAsync();
    }

}
