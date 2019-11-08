using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PartialPaymentAmount
    {
        public int paymentNumber { get; set; }
        public decimal amount { get; set; }
        public decimal onAccount { get; set; }
        public string description { get; set; }
        public string releaseDate { get; set; }
        public string paymentDay { get; set; }
    }
}
