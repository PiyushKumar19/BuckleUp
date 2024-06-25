using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BuckleUp.DatabaseContext
{
    // Middleware to set TenantInfo based on request headers
    public class TenantModMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMultiTenantContextAccessor<Tenant> _multiTenantContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantModMiddleware(RequestDelegate next,
                                   IMultiTenantContextAccessor<Tenant> multiTenantContextAccessor,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _multiTenantContextAccessor = multiTenantContextAccessor ?? throw new ArgumentNullException(nameof(multiTenantContextAccessor));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task Invoke(HttpContext context)
        {
            // Retrieve TenantId from request headers
            if (context.Request.Headers.TryGetValue("TenantId", out var tenantIdHeader))
            {
                var tenantId = tenantIdHeader.ToString();

                // Query the database to find the Tenant with the given tenantId
                var tenant = FindTenantById(tenantId); // Implement this method to find tenant by Id

                if (tenant != null)
                {
                    // Set TenantInfo in multi-tenant context
                    _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id = tenant.Id;
                    _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Identifier = tenant.Identifier;
                }
                else
                {
                    throw new InvalidOperationException($"Tenant with Id '{tenantId}' not found.");
                }
            }
            else
            {
                throw new InvalidOperationException("TenantId header not found in the request.");
            }

            await _next(context);
        }

        private Tenant FindTenantById(string tenantId)
        {
            // Implement logic to query and return the Tenant entity based on tenantId
            // Example:
            // return dbContext.Tenants.FirstOrDefault(t => t.Id == tenantId);
            throw new NotImplementedException();
        }
    }
}
