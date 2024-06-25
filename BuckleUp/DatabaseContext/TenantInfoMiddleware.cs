using BuckleUp.Models;
using Finbuckle.MultiTenant.Abstractions;

namespace BuckleUp.DatabaseContext
{
    public class TenantInfoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantInfoMiddleware> _logger;

        public TenantInfoMiddleware(RequestDelegate next, ILogger<TenantInfoMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IMultiTenantContextAccessor<Tenant> multiTenantContextAccessor)
        {
            var tenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
            if (tenantInfo != null)
            {
                _logger.LogInformation($"Tenant Info: Id={tenantInfo.Id}, Identifier={tenantInfo.Identifier}");
            }
            else
            {
                _logger.LogWarning("Tenant Info is null");
            }

            await _next(context);
        }
    }

    

}
