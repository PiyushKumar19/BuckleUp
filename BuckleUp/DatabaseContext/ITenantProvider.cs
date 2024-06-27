using BuckleUp.Models;

namespace BuckleUp.DatabaseContext
{
    public interface ITenantProvider
    {
        Tenant GetTenant();
    }

    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Tenant GetTenant()
        {
            return _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
        }
    }

}
