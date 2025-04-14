using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Enums;

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

    }
}
