using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Models.DTOs
{
    public class GroupedJobsDTO
    {
        public string Key { get; set; }
        public List<JobResponse> Jobs { get; set; } = new();

        public bool HasJobs => Jobs?.Any() == true;
    }
}
