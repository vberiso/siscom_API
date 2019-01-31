using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Tax_Address")]
    public class TaxAddress
    {
        [Column("id_tax_address"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Column("suburb"),StringLength(100)]
        public string Suburb { get; set; }
        [Column("town"),StringLength(30)]
        public string Town { get; set; }
        [Column("state"),StringLength(30)]
        public string State { get; set; }




        public int TaxUserId { get; set; }
        public TaxUser TaxUser { get; set; }




    }
}
