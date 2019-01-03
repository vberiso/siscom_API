using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionBranchOfficeVM
    {
        public string BranchOffice { get; set; }
        public int TerminalUserId { get; set; }
        public string Hour { get; set; }
        public Int16 Sign { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal Rounding { get; set; }
        public decimal Total { get; set; }
        public string TypeTransaction { get; set; }
        public string PayMethod { get; set; }
        public string Origin_Payment { get; set; }
        public string External_Origin_Payment { get; set; }
    }
}
