using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionDetailVM
    {
        [Required(ErrorMessage = "Concepto requerido")]
        public string CodeConcept { get; set; }      
        public string Description { get; set; }
        [Required(ErrorMessage = "Monto requerio")]
        public double amount { get; set; }
    }
}
