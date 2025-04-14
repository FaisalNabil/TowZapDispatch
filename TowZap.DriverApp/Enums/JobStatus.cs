using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Enums
{
    public enum JobStatus
    {
        Pending,        // Dispatcher just created it
        Assigned,       // Driver assigned
        EnRoute,        // Driver on the way
        Arrived,        // Driver arrived at pickup
        Towing,         // Vehicle is being towed to destination
        Completed,      // Job completed and verified
        Cancelled
    }
}
