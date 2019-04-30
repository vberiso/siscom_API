using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Model
{
    public class PaymentResume
    {
        public Payment payment { get; set; }
        public OrderSale orderSale { get; set; }
    }
}
