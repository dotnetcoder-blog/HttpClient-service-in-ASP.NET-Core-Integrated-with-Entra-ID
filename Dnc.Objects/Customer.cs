using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Objects
{
    public class Customer
    {
        public int Id { get; set; }            
        public string FirstName { get; set; }  
        public string LastName { get; set; }   
        public string Email { get; set; } 
        public IEnumerable<Order> Orders { get; set; }
    }
}
