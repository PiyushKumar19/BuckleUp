using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tenant = _tenantProvider.GetTenant();
            if (tenant == null)
            {
                Console.WriteLine("Warning: Tenant information is missing. Query filter for tenant-specific entities will not be applied.");
            }

            modelBuilder.Entity<User>()
            .HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .IsRequired();

            // Apply query filter for tenant-specific entities
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == tenant.Identifier);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == tenant.Id);
        }
    }

}
