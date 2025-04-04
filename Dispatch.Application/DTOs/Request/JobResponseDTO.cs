using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Request
{
    public class JobResponseDTO
    {
        public int Id { get; set; }

        // Caller & Account
        public string AccountName { get; set; }
        public string CallerName { get; set; }
        public string CallerPhone { get; set; }

        // Vehicle Info
        public string VIN { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public string StatePlate { get; set; }
        public string Keys { get; set; }
        public string UnitNumber { get; set; }
        public string Odometer { get; set; }

        // Tow Info
        public string Reason { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }

        // Assigned
        public string AssignedDriverId { get; set; }
        public string AssignedTowTruck { get; set; }

        // Meta
        public JobStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
