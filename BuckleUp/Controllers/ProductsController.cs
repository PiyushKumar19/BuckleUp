using BuckleUp.DatabaseContext;
using BuckleUp.Models;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;

        public ProductsController(AppDbContext db, IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor)
        {
            _db = db;
            _tenantContextAccessor = tenantContextAccessor;
        }
        // GET
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_db.Products.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        public IActionResult Create(List<Product> products)
        {
            var tenantInfo = _tenantContextAccessor.MultiTenantContext?.TenantInfo;
            if (tenantInfo == null)
            {
                return BadRequest("Tenant information is missing.");
            }

            // Set the TenantId based on the current tenant context
            foreach (var pro in products)
            {
                pro.TenantId = tenantInfo.Id;
                Console.WriteLine($"Creating product with TenantId: {pro.TenantId}");

            }
            //products.TenantId = tenantInfo.Id;

            // Log the tenant ID and entity state
            Console.WriteLine($"TenantInfo ID: {tenantInfo.Id}");
            Console.WriteLine($"TenantInfo Identifier: {tenantInfo.Identifier}");

            _db.Products.AddRange(products);

            // Check the entity state and log it
            //var entry = _db.Entry(products);
            //Console.WriteLine($"Entity State: {entry.State}");
            //Console.WriteLine($"Entity TenantId: {entry.Property(p => p.TenantId).CurrentValue}");

            _db.SaveChanges();
            return Ok(products);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _db.Products.Remove(_db.Products.FirstOrDefault(x => x.Id == id));
            _db.SaveChanges();
            return NoContent();
        }
    }
}
