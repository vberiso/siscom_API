using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TerminalVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mac Adress requerida")]
        public string MacAdress { get; set; }
        public decimal CashBox { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Sucursal requerida")]
        public int BranchOffice { get; set; }
    }
}
