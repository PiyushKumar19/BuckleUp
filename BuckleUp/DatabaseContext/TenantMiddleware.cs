using BuckleUp.Models;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.DatabaseContext
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantDbContext tenantDbContext)
        {
            var tenantClaim = context.User.FindFirst("TenantId").Value;
            if (tenantClaim is not null)
            {
                var tenant = await tenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Identifier == tenantClaim.ToString());
                if (tenant != null)
                {
                    context.Items["Tenant"] = tenant;
                }
                else
                {
                    Console.WriteLine("Invalid TenantId.");
                }
            }
            else
            {
                Console.WriteLine("TenantId header is missing.");
            }

            await _next(context);
        }
    }
 
}
