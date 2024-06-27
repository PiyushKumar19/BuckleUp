using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;

namespace BuckleUp.DatabaseContext
{
    public class TenantDbContext : EFCoreStoreDbContext<Tenant>
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
            // Ensure the database connection works
            TestDatabaseConnection();
        }

        public DbSet<Tenant> Tenants { get; set; }

        private void TestDatabaseConnection()
        {
            try
            {
                // Perform a simple query to test the connection
                this.Database.OpenConnection();
                this.Database.CloseConnection();
                Console.WriteLine("TenantDbContext connection established successfully.");
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Console.WriteLine($"TenantDbContext connection test failed: {ex.Message}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the table name if necessary
            modelBuilder.Entity<Tenant>().ToTable("Tenants");
        }
    }

}
