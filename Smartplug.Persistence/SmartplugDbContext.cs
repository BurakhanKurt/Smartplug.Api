using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smartplug.Core.Entity;
using Smartplug.Domain.Entities;
using System.Data;

namespace Smartplug.Persistence
{
    public class SmartplugDbContext : IdentityDbContext<Users, Role, Guid>
    {
        public SmartplugDbContext(DbContextOptions<SmartplugDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            
            //isDelete global query filter
            //.IgnoreQueryFilters()
            modelBuilder.Entity<Users>()
                .HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Role>()
                .HasQueryFilter(e => !e.IsDeleted);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<EnergyUsageLog> EnergyUsageLogs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is EntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((EntityBase)entity.Entity).CreatedAt = now;
                }
                ((EntityBase)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
