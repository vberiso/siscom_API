using Siscom.Agua.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Address")]
    public class Adress
    {
        [Column("id_adress"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("street"), StringLength(150), Required]
        public string Street { get; set; }
        [Column("outdoor"), StringLength(15), Required]
        public string Outdoor { get; set; }
        [Column("indoor"), StringLength(10), Required]
        public string Indoor { get; set; }
        [Column("zip"), StringLength(5), Required]
        public string Zip { get; set; }
        [Column("reference"), StringLength(200), Required]
        public string Reference { get; set; }
        [Column("lat"), StringLength(12)]
        public string Lat { get; set; }
        [Column("Lon"), StringLength(12)]
        public string Lon { get; set; }
        [Column("type_address"), StringLength(5)]
        public string TypeAddress { get; set; }

        [ForeignKey("Agreements")]
        public int AgreementsId { get; set; }
        public Agreement Agreements { get; set; }

        public Suburb Suburbs { get; set; }
        
    }
}
