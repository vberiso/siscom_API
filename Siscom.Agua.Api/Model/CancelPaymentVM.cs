using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class CancelPaymentVM
    {
        public TransactionVM Transaction { get; set; }
        public Payment Payment { get; set; }
    }
}
