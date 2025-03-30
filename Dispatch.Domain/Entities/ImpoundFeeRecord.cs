using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class ImpoundFeeRecord
    {
        public Guid Id { get; set; }
        public Guid JobRequestId { get; set; }
        public decimal TotalFee { get; set; }
        public DateTime CalculatedOn { get; set; }
    }
}
