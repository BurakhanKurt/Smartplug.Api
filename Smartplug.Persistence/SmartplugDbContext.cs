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


            modelBuilder.Entity<Users>().HasMany(u => u.Device).WithOne(d => d.User).HasForeignKey(d => d.UserId);
            modelBuilder.Entity<Device>().HasMany(d => d.EnergyUsageLogs).WithOne(e => e.Device).HasForeignKey(e => e.DeviceId);
            modelBuilder.Entity<Device>().HasMany(d => d.Schedules).WithOne(s => s.Device).HasForeignKey(s => s.DeviceId);
            modelBuilder.Entity<Device>().HasMany(d => d.DeviceLogs).WithOne(l => l.Device).HasForeignKey(l => l.DeviceId);
            modelBuilder.Entity<User>().HasMany(u => u.UserSettings).WithOne(s => s.User).HasForeignKey(s => s.UserId);

            //isDelete global query filter
            //.IgnoreQueryFilters()
            modelBuilder.Entity<Users>()
                .HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Role>()
                .HasQueryFilter(e => !e.IsDeleted);

        }

        public DbSet<Users> Users { get; set; }

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
