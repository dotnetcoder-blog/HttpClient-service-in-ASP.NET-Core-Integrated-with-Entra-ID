using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Objects
{
    public class Order
    {
        public int OrderId { get; set; } 
        public int CustomerId { get; set; } 
        public decimal TotalAmount { get; set; }
    }
}
