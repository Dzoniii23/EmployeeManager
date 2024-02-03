using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class OrderQueryModel
    {
        public int? EmpId { get; set; }
        public int? CustId { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public DateTime? RequiredDateFrom { get; set; }
        public DateTime? RequiredDateTo { get; set; }
        
        // Enum for order states
        public enum OrderState
        {
            Unknown = 0,
            InProduction = 1,
            ToBeShipped = 2,
            Shipped = 3,
            Closed = 4
        }
        public OrderState? State { get; set; }
    }
}
