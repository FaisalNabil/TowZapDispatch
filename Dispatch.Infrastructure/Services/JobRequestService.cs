using Dispatch.Application.Common.Interface;
using Dispatch.Application.DTOs.Request;
using Dispatch.Domain.Entities;
using Dispatch.Domain.Enums;
using Dispatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dispatch.Infrastructure.Services
{
    public class JobRequestService : IJobRequestService
    {
        private readonly ApplicationDbContext _context;

        public JobRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobResponseDTO>> GetJobsForDispatcherAsync(string dispatcherId)
        {
            return await _context.JobRequests
                .Where(j => j.CreatedById == dispatcherId)
                .Select(j => MapToDTO(j))
                .ToListAsync();
        }

        public async Task<List<JobResponseDTO>> GetJobsForDriverAsync(string driverId)
        {
            return await _context.JobRequests
                .Where(j => j.AssignedDriverId == driverId)
                .Select(j => MapToDTO(j))
                .ToListAsync();
        }

        public async Task<List<JobResponseDTO>> GetJobsForCompanyAsync(Guid companyId)
        {
            return await _context.JobRequests
                .Where(j => j.CompanyId == companyId)
                .Select(j => MapToDTO(j))
                .ToListAsync();
        }

        public async Task<JobResponseDTO?> GetJobByIdAsync(Guid id)
        {
            var job = await _context.JobRequests.FindAsync(id);
            return job == null ? null : MapToDTO(job);
        }

        public async Task<Guid> CreateJobAsync(CreateJobRequestDTO dto, string dispatcherId, Guid companyId)
        {
            var job = new JobRequest
            {
                AccountName = dto.AccountName,
                CallerName = dto.CallerName,
                CallerPhone = dto.CallerPhone,
                VIN = dto.VIN,
                Year = dto.Year,
                Make = dto.Make,
                Model = dto.Model,
                Color = dto.Color,
                PlateNumber = dto.PlateNumber,
                StatePlate = dto.StatePlate,
                Keys = dto.Keys,
                UnitNumber = dto.UnitNumber,
                Odometer = dto.Odometer,
                Reason = dto.Reason,
                FromLatitude = dto.FromLatitude,
                FromLongitude = dto.FromLongitude,
                ToLatitude = dto.ToLatitude,
                ToLongitude = dto.ToLongitude,
                AssignedDriverId = dto.AssignedDriverId,
                AssignedTowTruck = dto.AssignedTowTruck,
                Status = JobStatus.Assigned,
                CreatedById = dispatcherId,
                CreatedAt = DateTime.UtcNow,
                CompanyId = companyId
            };

            _context.JobRequests.Add(job);
            await _context.SaveChangesAsync();

            return job.Id;
        }

        public async Task<bool> UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return false;

            job.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignDriverAsync(Guid jobId, string driverUserId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return false;

            job.AssignedDriverId = driverUserId;
            job.Status = JobStatus.Assigned;

            _context.DriverStatuses.Add(new DriverStatus
            {
                Id = Guid.NewGuid(),
                JobRequestId = jobId,
                DriverUserId = driverUserId,
                Timestamp = DateTime.UtcNow,
                Status = "Assigned"
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(Guid jobId)
        {
            var job = await _context.JobRequests.FindAsync(jobId);
            if (job == null) return false;

            _context.JobRequests.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        private static JobResponseDTO MapToDTO(JobRequest j) => new()
        {
            Id = j.Id,
            CallerName = j.CallerName,
            CallerPhone = j.CallerPhone,
            Make = j.Make,
            Model = j.Model,
            PlateNumber = j.PlateNumber,
            Reason = j.Reason,
            FromLatitude = j.FromLatitude,
            FromLongitude = j.FromLongitude,
            ToLatitude = j.ToLatitude,
            ToLongitude = j.ToLongitude,
            AssignedDriverId = j.AssignedDriverId,
            AssignedTowTruck = j.AssignedTowTruck,
            Status = j.Status,
            CreatedAt = j.CreatedAt,
            CompanyId = j.CompanyId
        };
    }

}
