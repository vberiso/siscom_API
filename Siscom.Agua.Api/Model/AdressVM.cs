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
        [StringLength(150)]
        public string Street { get; set; }
        [StringLength(15)]
        public string Outdoor { get; set; }
        [StringLength(10)]
        public string Indoor { get; set; }
        [StringLength(5)]
        public string Zip { get; set; }
        [StringLength(200)]
        public string Reference { get; set; }
        [StringLength(12)]
        public string Lat { get; set; }
        [StringLength(12)]
        public string Lon { get; set; }
        [StringLength(5)]
        public string TypeAddress { get; set; }

        public int AgreementsId { get; set; }
        public int SuburbsId { get; set; }
    }
}
