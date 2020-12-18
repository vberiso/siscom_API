using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class FindAgreementParamVM
    {
        public int AgreementId { get; set; }
        public string Account { get; set; }
        public string Nombre { get; set; }
        public decimal taxableBase { get; set; }
        public int idClient { get; set; }
        public string RFC { get; set; }
        public string Address { get; set; }
        public bool WithDiscount { get; set; }
        public int idStus { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int    NumDerivades { get; set; }        
        public decimal Debit { get; set; }
        public string Token { get; set; }
        public string EndDate { get; set; }
        public string NameDiscount { get; set; }
        public bool? isActiveDiscount { get; set; }
    }
}
