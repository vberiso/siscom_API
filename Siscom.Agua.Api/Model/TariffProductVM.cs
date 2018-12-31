using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TariffProductVM
    {
        public int IdProduct { get; set; }
        public Int16 Type { get; set; }
        public decimal Account { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
