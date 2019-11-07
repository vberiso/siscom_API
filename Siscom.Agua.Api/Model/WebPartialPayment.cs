using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class WebPartialPayment
    {
        public int idAgreement { get; set; }
        public int numberPayments { get; set; }
        public string observations { get; set; }
        public string signatureName { get; set; }
        public string idNumber { get; set; }
        public string idCard { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string folioConvenio { get; set; }
        public string error { get; set; }
    }
}
