using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.SOSAPAC
{
    public class NotificationVM
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public DateTime NotificationDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime UntilDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Rounding { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public int AgreementId { get; set; }
    }
}
