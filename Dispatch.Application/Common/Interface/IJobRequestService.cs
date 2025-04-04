using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.Common.Interface
{
    public interface IJobRequestService
    {
        Task<bool> CreateJobAsync(CreateJobRequestDTO dto);
        Task<List<JobResponseDTO>> GetJobsForDispatcherAsync();
        Task<JobResponseDTO?> GetJobByIdAsync(int id);
        Task<bool> AssignDriverAsync(int jobId, string driverUserId); 
        Task<bool> UpdateJobStatusAsync(int jobId, JobStatus status);

    }
}
