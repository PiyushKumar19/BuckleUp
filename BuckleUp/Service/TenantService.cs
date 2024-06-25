//namespace BuckleUp.Service
//{
//    using BuckleUp.Models;
//    using Finbuckle.MultiTenant;
//    using Finbuckle.MultiTenant.Abstractions;

//    public class TenantService
//    {
//        private readonly IMultiTenantContextAccessor<Tenant> _tenantContextAccessor;
//        private readonly IMultiTenantContext<Tenant> _tenantContext;

//        public TenantService(IMultiTenantContextAccessor<Tenant> tenantContextAccessor)
//        {
//            _tenantContextAccessor = tenantContextAccessor;
//            _tenantContext = _tenantContextAccessor.MultiTenantContext;
//        }

//        public bool SetTenantInfo(Tenant tenantInfo)
//        {
//            return _tenantContext.TrySetTenantInfo(tenantInfo);
//        }
//    }

//}
