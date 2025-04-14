using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public decimal? TaxRate { get; set; }
        public int? StateId { get; set; }
        public Guid? ActiveSubscriptionId { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Telephone { get; set; }
        public string? OfficeHours { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Fax { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public string? Email { get; set; }
        public string? VSFLicense { get; set; }
        public string? Jurisdiction { get; set; }
        public string? DutyTypesConcat { get; set; }
        public int? TimeZoneId { get; set; }
        public string? ExternalCompanyId { get; set; }
        public string? ExternalSubscriptionId { get; set; }
        public bool? WizardCompleted { get; set; }
        public int? ProcessStepCompleted { get; set; }
        public string? CustomerPId { get; set; }
        public string? PToken { get; set; }
    }
}
