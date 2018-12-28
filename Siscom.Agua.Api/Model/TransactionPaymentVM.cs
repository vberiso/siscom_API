using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionPaymentVM
    {
        public Transaction Transaction { get; set; }
        public Payment Payment { get; set; }
    }
}
