using Blazored.LocalStorage;
using Dispatch.Application.DTOs.User;
using System.Net.Http.Json;

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
    }

}
