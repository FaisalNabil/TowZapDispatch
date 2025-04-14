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
        public DbSet<Company> Companies { get; set; }

        // Your domain entities
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<JobStatusHistory> JobStatusHistoies { get; set; }
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

            modelBuilder.Entity<ImpoundFeeRecord>()
                .Property(f => f.TotalFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<JobRequest>()
                .HasOne(j => j.AssignedDriver)
                .WithMany()
                .HasForeignKey(j => j.AssignedDriverId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete

            modelBuilder.Entity<JobRequest>()
                .HasOne(j => j.CreatedBy)
                .WithMany()
                .HasForeignKey(j => j.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete

        }
    }

}