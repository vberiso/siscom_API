using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Promos
{
    public class PeriodsDiscountCaja
    {
        public int año { get; set; }
        public List<int> meses { get; set; }
        public int Descuento { get; set; }
    }
}
