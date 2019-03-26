using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Model
{
    public class TransactionPaymentWithoutFacturaVM
    {
        public List<Transaction> lstTransaction { get; set; }
        public List<Payment> lstPayment { get; set; }
    }
}
