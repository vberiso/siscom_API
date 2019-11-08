using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PartialPaymentReceipts
    {
        public string type { get; set; }
        public decimal amount { get; set; }
        public string fromDate { get; set; }
        public string untilDate { get; set; }
    }
}
