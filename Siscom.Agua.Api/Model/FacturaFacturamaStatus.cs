using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class FacturaFacturamaStatus
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Folio { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
}
