using TowZap.DriverApp.Enums;

namespace TowZap.DriverApp.Models.DTOs
{
    public class DriverStatusHistoryItemDTO
    {
        public JobStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string DriverName { get; set; }
        public string? Note { get; set; }
    }
}
