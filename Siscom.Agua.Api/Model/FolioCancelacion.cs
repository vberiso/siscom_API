using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class FolioCancelacion
    {
        public string UUID { get; set; }
        public string ReceptorRFC { get; set; }
        public decimal Total { get; set; }
        public int PaymentId { get; set; }
        public int TaxReceiptId { get; set; }
    }
}
