using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Entities
{
    public class ApplicationRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; } = "UserRole";

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public class ApplicationUserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }

}
