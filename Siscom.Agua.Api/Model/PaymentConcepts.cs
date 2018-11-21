using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PaymentConcepts
    {
        public TransactionVM Transaction { get; set; }
        public List<TransactionDetailVM> Concepts { get; set; }
    }
}
