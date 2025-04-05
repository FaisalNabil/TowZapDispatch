using Blazored.LocalStorage;
using Dispatch.Application.DTOs.Registration;
using Dispatch.Application.DTOs.User;
using System.Net.Http.Json;
using TowZap.Client.Client.DTOs;

namespace TowZap.Client.Client.Service
{
    public class ClientUserService : BaseApiService, IClientUserService
    {
        public ClientUserService(HttpClient http, ILocalStorageService localStorage)
        : base(http, localStorage) { }

        public async Task<List<UserSummaryDTO>> GetUsersByCompanyAsync()
        {
            await AddBearerTokenAsync(); // Ensure token is added

            var result = await _http.GetFromJsonAsync<List<UserSummaryDTO>>("api/users/company-users");
            return result ?? new List<UserSummaryDTO>();
        }

        public async Task<bool> PromoteToDispatcherAsync(string userId)
        {
            await AddBearerTokenAsync(); // Ensure token is added

            var response = await _http.PostAsync($"api/users/promote-to-dispatcher?userId={userId}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> CreateUserByAdminAsync(AdminCreateUserDTO dto)
        {
            await AddBearerTokenAsync();
            var response = await _http.PostAsJsonAsync("api/users/admin-create-user", dto);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<DriverDropdownDTO>> GetDriversInCompanyAsync()
        {
            await AddBearerTokenAsync();
            var result = await _http.GetFromJsonAsync<List<DriverDropdownDTO>>("api/users/company-drivers");
            return result ?? new List<DriverDropdownDTO>();
        }


    }

}
