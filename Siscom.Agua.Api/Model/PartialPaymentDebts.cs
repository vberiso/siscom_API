using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PartialPaymentDebts
    {
        public decimal amount { get; set; }
        public decimal on_account { get; set; }
        public string nameConcept { get; set; }
        public decimal iva { get; set; }
    }
}
