using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Request
{
    public class DriverStatusHistoryItemDTO
    {
        public JobStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string DriverName { get; set; }
        public string? Note { get; set; }
    }
}
