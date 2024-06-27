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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tenant = _tenantProvider.GetTenant();
            if (tenant == null)
            {
                Console.WriteLine("Warning: Tenant information is missing. Query filter for tenant-specific entities will not be applied.");
            }

            modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == tenant.Identifier);
        }
    }

}
