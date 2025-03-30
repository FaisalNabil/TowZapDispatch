using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class NotificationLetter
    {
        public Guid Id { get; set; }
        public Guid JobRequestId { get; set; }
        public string LetterType { get; set; } // 1st or 2nd
        public DateTime GeneratedOn { get; set; }
        public string FilePath { get; set; }
    }
}
