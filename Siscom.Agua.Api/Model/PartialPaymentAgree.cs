using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PartialPaymentAgree
    {
        public string type { get; set; }
        public string fromDate { get; set; }
        public string untilDate { get; set; }
        public decimal amount { get; set; }
        public int year { get; set; }
        public int debtPeriodId { get; set; }
    }
}
