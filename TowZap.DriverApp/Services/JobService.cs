using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Models.DTOs;

namespace TowZap.DriverApp.Services
{
    public class JobService : BaseApiService
    {
        public JobService(HttpClient httpClient) : base(httpClient) { }

        public async Task<JobResponse?> GetCurrentJobAsync()
        {
            return await GetAsync<JobResponse>("jobrequests/driver/current");
        }

        public async Task<JobResponse?> GetJobByIdAsync(Guid jobId)
        {
            return await GetAsync<JobResponse>($"jobrequests/{jobId}");
        }

        public async Task<bool> UpdateJobStatusAsync(Guid jobId, JobStatus newStatus)
        {
            return await PutAsync($"jobrequests/{jobId}/status", newStatus);
        }
        public async Task<List<MetaEnumDTO>> GetJobStatusOptionsAsync()
        {
            return await GetAsync<List<MetaEnumDTO>>("meta/job-statuses") ?? new List<MetaEnumDTO>();
        }
        public async Task<List<DriverStatusHistoryItemDTO>> GetJobStatusHistoryAsync(Guid jobId)
        {
            return await GetAsync<List<DriverStatusHistoryItemDTO>>($"jobstatus/{jobId}/status-history")
                   ?? new List<DriverStatusHistoryItemDTO>();
        }
        public async Task<List<JobResponse>> GetJobsForDriverAsync()
        {
            return await GetAsync<List<JobResponse>>("jobrequests/driver") ?? new List<JobResponse>();
        }


    }
}
