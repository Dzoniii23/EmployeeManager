using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTables.Order>()
            .Property(o => o.OrderId)
            .UseIdentityColumn();

            modelBuilder.Entity<DbTables.Customer>()
            .Property(c => c.CustId)
            .UseIdentityColumn();

            //Modify OrderDetail class to include a composite key using a combination of data annotations and fluent API in Entity Framework Core.
            modelBuilder.Entity<DbTables.OrderDetail>()
                .HasKey(od => new {od.OrderId, od.ProductId});

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DbTables.Employee> Employees { get; set; }// = null!;
        public DbSet<DbTables.Supplier> Suppliers { get; set; }// = null!;
        public DbSet<DbTables.Category> Categories { get; set; }// = null!;
        public DbSet<DbTables.Product> Products { get; set; }// = null!;
        public DbSet<DbTables.Customer> Customers { get; set; }// = null!;
        public DbSet<DbTables.Shipper> Shippers { get; set; }// = null!;
        public DbSet<DbTables.Order> Orders { get; set; }// = null!;
        public DbSet<DbTables.OrderDetail> OrderDetails { get; set; }// = null!;

        public ApplicationContext()
        {
        }
        
    }    
}
