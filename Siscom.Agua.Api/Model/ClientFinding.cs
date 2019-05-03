using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ClientFinding
    {
        public int Id { get; set; }        
        public string Name { get; set; }        
        public string LastName { get; set; }        
        public string SecondLastName { get; set; }        
        public string RFC { get; set; }

        public string Street { get; set; }        
        public string Outdoor { get; set; }        
        public string Indoor { get; set; }
        public string Suburb { get; set; }
        public string Zip { get; set; }
        public string Reference { get; set; }
        public string Town { get; set; }
        public string State { get; set; }

        public ICollection<Agreement> agreements { get; set; }
        public ICollection<OrderSale> orderSales { get; set; }
        
    }
}
