using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class JobRequest
    {
        public Guid Id { get; set; }

        // Caller & Account
        public string AccountName { get; set; } = string.Empty;
        public string CallerName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty;

        // Vehicle Info
        public string VIN { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string StatePlate { get; set; } = string.Empty;
        public string Keys { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string Odometer { get; set; } = string.Empty;

        // Reason
        public string Reason { get; set; } = string.Empty;

        // Location
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }


        // Assignment
        public string AssignedDriverId { get; set; }
        public ApplicationUser AssignedDriver { get; set; }

        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string AssignedTowTruck { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CompanyId { get; set; }  // For multi-tenancy
    }

}
