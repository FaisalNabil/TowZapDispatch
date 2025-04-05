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
        Task<List<JobResponseDTO>> GetJobsForDispatcherAsync(string dispatcherId);
        Task<List<JobResponseDTO>> GetJobsForDriverAsync(string driverId);
        Task<List<JobResponseDTO>> GetJobsForCompanyAsync(Guid companyId);
        Task<JobResponseDTO?> GetJobByIdAsync(Guid id);
        Task<Guid> CreateJobAsync(CreateJobRequestDTO dto, string dispatcherId, Guid companyId);
        Task<bool> UpdateJobStatusAsync(Guid jobId, JobStatus status);
        Task<bool> AssignDriverAsync(Guid jobId, string driverUserId);
        Task<bool> DeleteJobAsync(Guid jobId);

    }
}
