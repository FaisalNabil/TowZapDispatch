using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Enums
{
    public enum JobStatus
    {
        [Description("Job Created")]
        Pending,

        [Description("Driver Assigned")]
        Assigned,

        [Description("Driver En Route")]
        EnRoute,

        [Description("Driver Arrived")]
        Arrived,

        [Description("Towing in Progress")]
        Towing,

        [Description("Job Completed")]
        Completed,

        [Description("Cancelled by Dispatcher")]
        Cancelled,

        [Description("Declined by Driver")]
        Declined
    }
}
