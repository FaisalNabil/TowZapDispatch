﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string CompanyName { get; set; }
        public string UserId { get; set; }
    }
}
