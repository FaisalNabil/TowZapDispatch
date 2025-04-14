using Blazored.LocalStorage;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using System.Net.Http.Json;
using TowZap.Client.Client.DTOs;

namespace TowZap.Client.Client.Service
{
    public class ClientJobRequestService : BaseApiService, IClientJobRequestService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public ClientJobRequestService(HttpClient http, ILocalStorageService localStorage)
            : base(http, localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<bool> CreateJobAsync(CreateJobRequestDTO dto)
        {
            await AddBearerTokenAsync();
            var response = await _http.PostAsJsonAsync("api/JobRequests/create", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + errorContent); // or log it in UI
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<JobResponseDTO>> GetJobsForDispatcherAsync()
        {
            await AddBearerTokenAsync();
            var jobs = await _http.GetFromJsonAsync<List<JobResponseDTO>>("api/JobRequests/dispatcher");
            return jobs ?? new List<JobResponseDTO>();
        }

        public async Task<List<JobResponseDTO>> GetJobsForDriverAsync()
        {
            await AddBearerTokenAsync();
            var jobs = await _http.GetFromJsonAsync<List<JobResponseDTO>>("api/JobRequests/driver");
            return jobs ?? new List<JobResponseDTO>();
        }

        public async Task<List<JobResponseDTO>> GetJobsForGuestAsync()
        {
            await AddBearerTokenAsync();
            var jobs = await _http.GetFromJsonAsync<List<JobResponseDTO>>("api/JobRequests/guest");
            return jobs ?? new List<JobResponseDTO>();
        }

        public async Task<JobResponseDTO?> GetJobByIdAsync(Guid id)
        {
            await AddBearerTokenAsync();
            return await _http.GetFromJsonAsync<JobResponseDTO>($"api/JobRequests/{id}");
        }
        public async Task<List<DriverStatusHistoryItemDTO>> GetJobStatusHistoryAsync(Guid jobId)
        {
            await AddBearerTokenAsync(); 
            var response = await _http.GetFromJsonAsync<List<DriverStatusHistoryItemDTO>>($"api/JobStatus/{jobId}/status-history");
            return response ?? new List<DriverStatusHistoryItemDTO>();
        }


        public async Task<bool> AssignDriverAsync(Guid jobId, string driverUserId)
        {
            await AddBearerTokenAsync();
            var response = await _http.PostAsync($"api/JobRequests/{jobId}/assign?driverUserId={driverUserId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateJobStatusAsync(Guid jobId, JobStatus newStatus)
        {
            await AddBearerTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/JobRequests/{jobId}/status", newStatus);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<MetaEnumDTO>> GetJobStatusesAsync()
        {
            await AddBearerTokenAsync();
            var result = await _http.GetFromJsonAsync<List<MetaEnumDTO>>("api/meta/job-statuses");
            return result ?? new List<MetaEnumDTO>();
        }
        public async Task<JobResponseDTO?> GetCurrentJobForDriverAsync()
        {
            await AddBearerTokenAsync();
            return await _http.GetFromJsonAsync<JobResponseDTO>("api/JobRequests/driver/current");
        }
    }
}
