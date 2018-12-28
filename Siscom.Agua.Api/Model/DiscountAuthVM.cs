using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class DiscountAuthVM
    {
        public int AgreementId { get; set; }
        public int DebtId { get; set; }
        public int Porcentage { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
