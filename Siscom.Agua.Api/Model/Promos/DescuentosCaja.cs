using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Promos
{
    public class DescuentosCaja
    {
        public List<string> tipos { get; set; }
        public List<int> codes { get; set; }
        public int descuento { get; set; }
        public DateTime condonacionDeudaDesde { get; set; }
        public DateTime condonacionDeudaHasta { get; set; }
    }
}
