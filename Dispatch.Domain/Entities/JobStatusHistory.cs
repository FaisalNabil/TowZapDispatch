using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class JobStatusHistory
    {
        public Guid Id { get; set; }
        public JobStatus Status { get; set; } // Enroute, On-site, etc.
        public DateTime Timestamp { get; set; }
        public Guid JobRequestId { get; set; }
        public JobRequest JobRequest { get; set; }
        public string UpdatedByUserId { get; set; }
        public ApplicationUser UpdatedByUser { get; set; }
        public string? Note { get; set; }
    }
}
