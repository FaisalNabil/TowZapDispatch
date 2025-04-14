using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Enums;
using TowZap.Client.Client.DTOs;

namespace TowZap.Client.Client.Service
{
    public interface IClientJobRequestService
    {
        Task<bool> CreateJobAsync(CreateJobRequestDTO dto);
        Task<List<JobResponseDTO>> GetJobsForDispatcherAsync();
        Task<JobResponseDTO?> GetJobByIdAsync(Guid id);
        Task<List<DriverStatusHistoryItemDTO>> GetJobStatusHistoryAsync(Guid jobId);
        Task<bool> AssignDriverAsync(Guid jobId, string driverUserId);
        Task<bool> UpdateJobStatusAsync(Guid jobId, JobStatus newStatus);
        Task<List<JobResponseDTO>> GetJobsForDriverAsync();
        Task<List<JobResponseDTO>> GetJobsForGuestAsync();
        Task<List<MetaEnumDTO>> GetJobStatusesAsync();
        Task<JobResponseDTO?> GetCurrentJobForDriverAsync();
    }
}
