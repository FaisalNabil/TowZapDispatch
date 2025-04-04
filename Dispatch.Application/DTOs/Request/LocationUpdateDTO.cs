using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Request
{
    public class LocationUpdateDTO
    {
        public int JobRequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
