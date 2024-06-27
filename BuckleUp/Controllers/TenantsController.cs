using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuckleUp.DatabaseContext;
using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMultiTenantStore<Tenant> _tenantStore;
    private readonly ApplicationDbContext _db;

    public TenantsController(IMultiTenantStore<Tenant> tenantStore, ApplicationDbContext db)
    {
        _tenantStore = tenantStore;
        _db = db;
    }

    // GET: api/tenants
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
    {
        try
        {
            var tenants = await _tenantStore.GetAllAsync();
            return Ok(tenants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: api/tenants/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tenant>> GetTenant(string id)
    {
        try
        {
            var tenant = await _tenantStore.TryGetAsync(id);
            if (tenant == null)
            {
                return NotFound($"Tenant with ID '{id}' not found.");
            }
            return Ok(tenant);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/tenants
    // POST: api/tenants
    [HttpPost]
    public async Task<ActionResult<TenantInfo>> CreateTenant(Tenant tenantInfo)
    {
        try
        {
            //var success = await _tenantStore.TryAddAsync(tenantInfo);
            var existing = await _tenantStore.TryGetByIdentifierAsync(tenantInfo.Identifier);

            var newTenant = await _db.Tenants.AddAsync(tenantInfo);
            await _db.SaveChangesAsync();
            var tenant = await _tenantStore.TryGetByIdentifierAsync(tenantInfo.Identifier);
            if (existing is not null)
            {
                return Conflict($"Tenant with ID '{tenant.Id}' or Identifier '{tenant.Identifier}' already exists.");
            }
            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Exception occurred while creating tenant: {ex}");

            // Return a 500 Internal Server Error response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    // PUT: api/tenants/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(string id, Tenant tenantInfo)
    {
        try
        {
            if (id != tenantInfo.Id)
            {
                return BadRequest("Tenant ID mismatch.");
            }

            var success = await _tenantStore.TryUpdateAsync(tenantInfo);
            if (!success)
            {
                return NotFound($"Tenant with ID '{id}' not found.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DELETE: api/tenants/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(string id)
    {
        try
        {
            var success = await _tenantStore.TryRemoveAsync(id);
            if (!success)
            {
                return NotFound($"Tenant with ID '{id}' not found.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
