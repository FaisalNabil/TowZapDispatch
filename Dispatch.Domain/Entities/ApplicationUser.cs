using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Dispatch.Domain.Entities
{
    public class ApplicationUser
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public required string UserName { get; set; }
        public Guid CompanyId { get; set; }

        [NotMapped] 
        public string? CompanyName { get; set; }
        public string? TdlrLicense { get; set; }
        public Guid SubscriptionPackageId { get; set; }
        public int SubscriptionPackageNumber { get; set; }
        public bool IsDisabled { get; set; }
        public string? AccessNote { get; set; }
        public bool HasAccessGranted { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime? SubscriptionExpiresOn { get; set; }

        public ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }

}
