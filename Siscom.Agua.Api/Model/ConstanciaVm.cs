using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ConstanciaVm
    {
        public int Id { get; set; }
        [StringLength(400), Required]
        public string Description { get; set; }

    }

   
}
