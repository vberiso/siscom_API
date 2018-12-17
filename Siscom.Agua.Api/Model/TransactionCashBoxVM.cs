using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionCashBoxVM
    {
        public bool Sign { get; set; }
        public decimal Amount { get; set; }      
        public string Aplication { get; set; }
        public int TypeTransactionId { get; set; }
        public int PayMethodId { get; set; }
        public int TerminalUserId { get; set; }   
    }
}
