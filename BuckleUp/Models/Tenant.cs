using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;

namespace BuckleUp.Models
{
    public class Tenant : ITenantInfo
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
    }

}
