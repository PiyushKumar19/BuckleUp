using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.DatabaseContext
{
    public class AppDbContext : MultiTenantDbContext
    {
        private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;


        public DbSet<Product> Products { get; set; }

        public AppDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions<AppDbContext> options) 
            : base(multiTenantContextAccessor, options)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor ?? throw new ArgumentNullException(nameof(multiTenantContextAccessor));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                return tenantContext.TenantInfo.Id; // Assuming TenantInfo has an Id property
            }

            // Handle case where tenant context or tenant info is null
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
