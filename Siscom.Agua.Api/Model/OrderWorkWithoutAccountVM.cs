using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class OrderWorkWithoutAccountVM
    {
        public OrderWork OrderWork { get; set; }
        public TaxUser TaxUser { get; set; }
    }
}
