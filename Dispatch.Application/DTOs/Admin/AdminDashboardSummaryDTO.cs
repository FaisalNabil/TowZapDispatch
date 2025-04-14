using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Admin
{
    public class AdminDashboardSummaryDTO
    {
        public int TotalUsers { get; set; }
        public int DispatcherCount { get; set; }
        public int DriverCount { get; set; }
        public int GuestCount { get; set; }

        public int TotalJobs { get; set; }
        public List<JobStatusCountDTO> JobStatusCounts { get; set; } = new();

        public List<RecentJobDTO> RecentJobs { get; set; } = new();
    }

    public class JobStatusCountDTO
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class RecentJobDTO
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; }
        public string Status { get; set; }
        public string DriverName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
