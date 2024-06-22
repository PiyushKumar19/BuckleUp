namespace BuckleUp.Models
{
    using Finbuckle.MultiTenant;
    using System.ComponentModel.DataAnnotations;

    [MultiTenant]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //[Required]
        public string? TenantId { get; set; }
    }

}
