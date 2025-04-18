﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class JobLocationUpdate
    {
        public int Id { get; set; }
        public int JobRequestId { get; set; }
        public JobRequest JobRequest { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Guid UpdatedByDriverId { get; set; }
    }

}
