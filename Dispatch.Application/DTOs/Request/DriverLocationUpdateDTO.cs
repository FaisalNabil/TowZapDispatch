using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Request
{
    public class DriverLocationUpdateDTO
    {
        public Guid JobRequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }  // Optional: EnRoute, Reached, etc.
    }

}
