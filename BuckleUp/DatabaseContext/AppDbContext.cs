using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.DatabaseContext
{
    public class AppDbContext : MultiTenantDbContext
    {
        private readonly IMultiTenantContextAccessor<Tenant> _multiTenantContextAccessor;


        public DbSet<Product> Products { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        public AppDbContext(IMultiTenantContextAccessor<Tenant> multiTenantContextAccessor, DbContextOptions<AppDbContext> options) 
            : base(multiTenantContextAccessor, options)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor ?? throw new ArgumentNullException(nameof(multiTenantContextAccessor));

            var tenant = _multiTenantContextAccessor.MultiTenantContext;

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tenant = _multiTenantContextAccessor.MultiTenantContext;

            var productEntity = modelBuilder.Entity<Product>();

            productEntity.Property(p => p.TenantId).IsRequired();
            modelBuilder.ConfigureMultiTenant();
            productEntity.IsMultiTenant();

        }

        private string GetCurrentTenantId()
        {
            var tenantContext = _multiTenantContextAccessor.MultiTenantContext;

            if (tenantContext?.TenantInfo != null)
            {
                return tenantContext.TenantInfo.Id; 
            }
            throw new InvalidOperationException("Unable to determine current tenant.");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var tenant = this.GetCurrentTenantId();
            this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
