using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class DriverStatus
    {
        public Guid Id { get; set; }
        public string Status { get; set; } // Enroute, On-site, etc.
        public DateTime Timestamp { get; set; }
        public Guid JobRequestId { get; set; }
        public JobRequest JobRequest { get; set; }
        public string DriverUserId { get; set; }
        public ApplicationUser DriverUser { get; set; }
    }
}
