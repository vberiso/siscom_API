using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementDiscounttVM
    {
        [Required]
        public int AgreementId { get; set; }
        [Required]
        public int DiscountId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
