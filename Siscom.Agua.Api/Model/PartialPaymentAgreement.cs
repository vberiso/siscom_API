using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PartialPaymentAgreement
    {
        public int idPartialPayment { get; set; }
        public string folio { get; set; }
        public string partialPaymentDate { get; set; }
        public decimal amount { get; set; }
        public int numberPayments { get; set; }
        public string description { get; set; }
        public string expiration_date { get; set; }
        public int AgreementId { get; set; }
        public int Account { get; set; }
    }
}
