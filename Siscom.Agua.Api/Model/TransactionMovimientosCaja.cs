using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionMovimientosCaja
    {
        public int IdTransaction { get; set; }
        public string FolioTransaccion { get; set; }
        public int IdPayment { get; set; }
        public string Cliente { get; set; }
        public string Cuenta { get; set; }
        public string Operacion { get; set; }
        public string FolioImpresion { get; set; }        
        public string Hora { get; set; }
        public decimal Total { get; set; }
        public bool Signo { get; set; }
        public bool HaveInvoice { get; set; }
    }
}
