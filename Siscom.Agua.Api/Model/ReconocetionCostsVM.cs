using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ReconocetionCostsVM
    {
        public int IdReconection { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public bool HaveTax { get; set; }
        public string Type { get; set; }
        public string TypeIntake { get; set; }
    }
}
