using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class OrderEditReport
    {
        #region General order details
        public int OrderId { get; set; }
        public int State { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int EmpId { get; set; }
        #endregion

        #region Customer details
        public int? CustId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        #endregion

        #region Shiping to details
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        #endregion

        #region Products details
        public List<ProductInOrder>? ProductsInOrder { get; set; }
        #endregion
    }

    public class ProductInOrder
    {
        public int ProductId { get; set;}
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short Qty { get; set; }
        public decimal Discount { get; set; }
        public bool Discontinued { get; set; }
        public decimal Stock { get; set; }
    }

    public class ProductToAdd
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public decimal Stock { get; set; }
    }
}
