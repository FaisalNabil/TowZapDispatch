using Dispatch.Application.DTOs.User;

namespace TowZap.Client.Client.Service
{
    public interface IClientUserService
    {
        Task<List<UserSummaryDTO>> GetUsersByCompanyAsync();
        Task<bool> PromoteToDispatcherAsync(string userId);
    }

}
