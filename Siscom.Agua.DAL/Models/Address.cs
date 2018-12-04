using Siscom.Agua.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Address")]
    public class Address
    {
        [Column("id_address"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("street"), StringLength(150)]
        public string Street { get; set; }
        [Column("outdoor"), StringLength(50), Required]
        public string Outdoor { get; set; }
        [Column("indoor"), StringLength(50)]
        public string Indoor { get; set; }
        [Column("zip"), StringLength(5)]
        public string Zip { get; set; }
        [Column("reference"), StringLength(200), Required]
        public string Reference { get; set; }
        [Column("lat"), StringLength(20)]
        public string Lat { get; set; }
        [Column("Lon"), StringLength(20)]
        public string Lon { get; set; }
        [Column("type_address"), StringLength(5)]
        public string TypeAddress { get; set; }

        //[ForeignKey("Agreements")]
        public int AgreementsId { get; set; }
        public Agreement Agreements { get; set; }

        //[ForeignKey("Suburbs")]
        public int SuburbsId { get; set; }
        public Suburb Suburbs { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Street, Indoor);
        }

    }
}
