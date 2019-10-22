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
        public string RFC { get; set; }
        public string Address { get; set; }
        public bool WithDiscount { get; set; }
        public int idStus { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int    NumDerivades { get; set; }
        public decimal Debit { get; set; }
    }
}
