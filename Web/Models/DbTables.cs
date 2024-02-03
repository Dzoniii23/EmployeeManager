using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class DbTables
    {
        [Table("Employees", Schema = "HR")]
        public class Employee
        {
            [Key]
            public int EmpId { get; set; }
            public string? LastName { get; set; }
            public string? FirstName { get; set; }
            public string? Title { get; set; }
            public string? TitleOfCourtesy { get; set; }
            public DateTime BirthDate { get; set; }
            public DateTime HireDate { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? PostalCode { get; set; }
            public string? Country { get; set; }
            public string? Phone { get; set; }
            public string? Role { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? PhotoName { get; set; }
            public bool PassNotChanged { get; set; }
            public int? MgrId { get; set; }
        }
        [Table("Customers", Schema = "Sales")]
        public class Customer
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int CustId { get; set; }
            public string? CompanyName { get; set; }
            public string? ContactName { get; set; }
            public string? ContactTitle { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? PostalCode { get; set; }
            public string? Country { get; set; }
            public string? Phone { get; set; }
            public string? Fax { get; set; }
        }
        [Table("Orders", Schema = "Sales")]
        public class Order
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int OrderId { get; set; }
            public int State { get; set; }
            public int? CustId { get; set; }
            public int EmpId { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime RequiredDate { get; set; }
            public DateTime? ShippedDate { get; set; }
            public int ShipperId { get; set; }
            public decimal Freight { get; set; }
            public string? ShipName { get; set; }
            public string? ShipAddress { get; set; }
            public string? ShipCity { get; set; }
            public string? ShipRegion { get; set; }
            public string? ShipPostalCode { get; set; }
            public string? ShipCountry { get; set; }

        }
        [Table("OrderDetails", Schema = "Sales")]
        public class OrderDetail
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public decimal UnitPrice { get; set; }
            public short Qty { get; set; }
            public decimal Discount { get; set; }
        }
        [Table("Products", Schema = "Production")]
        public class Product
        {
            [Key]
            public int ProductId { get; set; }
            public string? ProductName { get; set; }
            public int SupplierId { get; set; }
            public int CategoryId { get; set; }
            public decimal UnitPrice { get; set; }
            public bool Discontinued { get; set; }
            public decimal Stock { get; set; }
        }
        [Table("Categories", Schema = "Production")]
        public class Category
        {
            [Key]
            public int CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? Description { get; set; }
        }
        [Table("Shippers", Schema = "Sales")]
        public class Shipper
        {
            [Key]
            public int ShipperId { get; set; }
            public string? CompanyName { get; set; }
            public string? Phone { get; set; }
        }
        [Table("Suppliers", Schema = "Production")]
        public class Supplier
        {
            [Key]
            public int SupplierId { get; set; }
            public string? CompanyName { get; set; }
            public string? ContactName { get; set; }
            public string? ContactTitle { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? PostalCode { get; set; }
            public string? Country { get; set; }
            public string? Phone { get; set; }
            public string? Fax { get; set; }
        }


    }
}
