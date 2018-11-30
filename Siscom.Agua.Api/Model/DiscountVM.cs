using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class DiscountVM
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        [Required]
        public Int16 Percentage { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int TypePeriodId { get; set; }
    }
}
