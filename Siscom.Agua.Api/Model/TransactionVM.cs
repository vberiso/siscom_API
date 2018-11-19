using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionVM
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public DateTime DateTransaction { get; set; }
        public bool Sign { get; set; }
        public double Amount { get; set; }
        public string Aplication { get; set; }
        public int TypeTransactionId { get; set; }
        public int PayMethodId { get; set; }
        public int TerminalUserId { get; set; }
    }
}
