using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Infrastructure.Services
{
    public class JobRequestService : IJobRequestService
    {
        private readonly HttpClient _http;

        public JobRequestService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CreateJobAsync(CreateJobRequestDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/JobRequests", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<JobResponseDTO>> GetJobsForDispatcherAsync()
        {
            var response = await _http.GetFromJsonAsync<List<JobResponseDTO>>("api/JobRequests/dispatcher");
            return response ?? new List<JobResponseDTO>();
        }

        public async Task<JobResponseDTO?> GetJobByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<JobResponseDTO>($"api/JobRequests/{id}");
        }

        public async Task<bool> AssignDriverAsync(int jobId, string driverUserId)
        {
            var response = await _http.PostAsync($"api/JobRequests/{jobId}/assign?driverUserId={driverUserId}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateJobStatusAsync(int jobId, JobStatus status)
        {
            var response = await _http.PutAsJsonAsync($"api/JobRequests/{jobId}/status", status);
            return response.IsSuccessStatusCode;
        }

    }
}
