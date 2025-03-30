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
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public string VIN { get; set; }
        public string Color { get; set; }
        public string Problem { get; set; }
        public string Authorization { get; set; }
        public string CustomerPhone { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public DateTime TowDate { get; set; }
        public string TowTime { get; set; }
        public decimal TowAmount { get; set; }
        public string AccountName { get; set; }
        public string Notes { get; set; }
        public string PhotoPath { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public bool IsImpoundTriggered { get; set; } = false;
        public bool IsFeeCalculated { get; set; } = false;
        public DateTime? FeeCalculationDate { get; set; }
        public double? LatitudeFrom { get; set; }
        public double? LongitudeFrom { get; set; }
        public double? LatitudeTo { get; set; }
        public double? LongitudeTo { get; set; }

    }
}
