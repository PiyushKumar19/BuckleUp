using BuckleUp.DatabaseContext;
using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CustomTenantStore : IMultiTenantStore<Tenant>
{
    private readonly AppDbContext _dbContext;
    private readonly IMultiTenantContextAccessor<Tenant> _multiTenantContextAccessor;


    public CustomTenantStore(AppDbContext dbContext, IMultiTenantContextAccessor<Tenant> multiTenantContextAccessor)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _multiTenantContextAccessor = multiTenantContextAccessor;

        var tenant = _multiTenantContextAccessor.MultiTenantContext;

    }

    public async Task<Tenant> TryGetAsync(string id)
    {
        return await _dbContext.Tenants.FindAsync(id);
    }

    public async Task<Tenant> TryGetByIdentifierAsync(string identifier)
    {
        var tenantInfo = _multiTenantContextAccessor.MultiTenantContext;

        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Identifier == identifier);
        var tenantIn = _multiTenantContextAccessor.MultiTenantContext;

        return tenant;
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync()
    {
        var tenants = await _dbContext.Tenants
            .IgnoreQueryFilters().ToListAsync();
        return tenants;
    }

    public async Task<bool> TryAddAsync(Tenant tenantInfo)
    {
        if (_dbContext.Tenants.Any(t => t.Id == tenantInfo.Id || t.Identifier == tenantInfo.Identifier))
        {
            return false; // Tenant with same Id or Identifier already exists
        }

        _dbContext.Tenants.Add(tenantInfo);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryUpdateAsync(Tenant tenantInfo)
    {
        var existingTenant = await _dbContext.Tenants.FindAsync(tenantInfo.Id);
        if (existingTenant == null)
        {
            return false; // Tenant not found
        }

        existingTenant.Identifier = tenantInfo.Identifier;
        existingTenant.Name = tenantInfo.Name;
        // Update other properties as needed

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveAsync(string id)
    {
        var tenantToRemove = await _dbContext.Tenants.FindAsync(id);
        if (tenantToRemove == null)
        {
            return false; // Tenant not found
        }

        _dbContext.Tenants.Remove(tenantToRemove);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
