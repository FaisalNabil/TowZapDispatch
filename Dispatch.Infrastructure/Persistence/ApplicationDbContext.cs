using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dispatch.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dispatch.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<ApplicationUserRole> UserRoles { get; set; }

        // Your domain entities
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<DriverStatus> DriverStatuses { get; set; }
        public DbSet<ImpoundFeeRecord> ImpoundFees { get; set; }
        public DbSet<NotificationLetter> NotificationLetters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers").HasKey(u => u.Id);
            modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles").HasKey(r => r.Id);
            modelBuilder.Entity<ApplicationUserRole>().ToTable("AspNetUserRoles").HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Example precision setup
            modelBuilder.Entity<JobRequest>()
                .Property(j => j.TowAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ImpoundFeeRecord>()
                .Property(f => f.TotalFee)
                .HasPrecision(18, 2);
        }
    }

}