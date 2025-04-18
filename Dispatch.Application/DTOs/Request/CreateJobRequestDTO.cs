﻿using Dispatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.DTOs.Request
{
    public class CreateJobRequestDTO
    {
        // Caller & Account
        public string AccountName { get; set; }
        public string CallerName { get; set; }
        public string CallerPhone { get; set; }

        // Vehicle Info
        public string VIN { get; set; }
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Enter a valid 4-digit year")]
        public string Year { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public string StatePlate { get; set; }
        public string Keys { get; set; } // Yes, No
        public string UnitNumber { get; set; }
        public string Odometer { get; set; }

        // Tow Reason
        public string Reason { get; set; } // e.g., Normal, Abandoned, Accident, etc.

        // Location
        public string FromLocation { get; set; }
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public string ToLocation { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }


        // Assignment
        public string AssignedDriverId { get; set; }
        public string AssignedTowTruck { get; set; }
    }

    public class CallerInfoDTO
    {
        public string CallerName { get; set; }
        public string PhoneNumber { get; set; }
    }


    public class LocationResult
    {
        public string Address { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
