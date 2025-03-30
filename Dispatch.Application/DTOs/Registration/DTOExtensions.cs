using Dispatch.Domain.Entities;
using System.ComponentModel.Design;
using System.Numerics;

namespace Dispatch.Application.DTOs.Registration
{
    public static class DTOExtensions
    {
        public static ApplicationUser ToUser(this DriverRegistrationRequestDTO dto) => new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FullName,
            LastName = "",
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true,
            CompanyId = Guid.Empty,

            // Required Identity fields
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

        public static ApplicationUser ToUser(this DispatcherRegistrationDTO dto) => new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            FirstName = dto.FullName,
            EmailConfirmed = true
        };

        public static ApplicationUser ToUser(this GuestRegistrationDTO dto) => new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FullName,
            CompanyId = dto.CompanyId,
            EmailConfirmed = true
        };
    }
}
