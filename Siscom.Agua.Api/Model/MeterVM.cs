using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class MeterVM
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Brand { get; set; }
        [StringLength(50), Required]
        public string Model { get; set; }
        [StringLength(10)]
        public string Consumption { get; set; }
        [Required]
        public DateTime InstallDate { get; set; }
        [Required]
        public DateTime DeinstallDate { get; set; }
        [StringLength(20), Required]
        public string Serial { get; set; }
        [StringLength(1), Required]
        public string Wheels { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int AgreementId { get; set; }
    }
}
