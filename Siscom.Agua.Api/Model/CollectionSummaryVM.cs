using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class CollectionSummaryVM
    {
        public string PaymentDate { get; set; }
        public string Account { get; set; }
        public string Client { get; set; }
        public decimal Total { get; set; }
        public string BrancOffice { get; set; }
        public string PayMethod { get; set; }
        public string OriginPayment { get; set; }
        public string External_Origin_Payment { get; set; }
        public string Folio { get; set; }
        public string User { get; set; }
    }
}
