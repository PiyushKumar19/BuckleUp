using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuckleUp.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Ulid.NewUlid().ToString(); public string Name { get; set; }
        public string? Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }
        public string? TenantId { get; set; }
    }
}
