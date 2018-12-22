using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AdressVM
    {
        public int Id { get; set; }
        [StringLength(150),Required]
        public string Street { get; set; }
        [StringLength(15), Required]
        public string Outdoor { get; set; }
        [StringLength(10)]
        public string Indoor { get; set; }
        [StringLength(5), Required]
        public string Zip { get; set; }
        [StringLength(200)]
        public string Reference { get; set; }
        [StringLength(20)]
        public string Lat { get; set; }
        [StringLength(20)]
        public string Lon { get; set; }
        [StringLength(5), Required]
        public string TypeAddress { get; set; }
        [Required]
        public int SuburbsId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
