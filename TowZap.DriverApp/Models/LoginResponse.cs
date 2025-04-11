using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}
