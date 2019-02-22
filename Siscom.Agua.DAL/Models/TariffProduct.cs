using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{ 
    [Table("Tariff_Product")]
    public class TariffProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_tariff")]
        public int Id { get; set; }      
        [Column("account_number"), StringLength(20), Required]
        public string AccountNumber { get; set; }
        [Column("unit_measurement"), StringLength(10), Required]
        public string UnitMeasurement { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }       
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("from_date"), Required]
        public DateTime FromDate { get; set; }
        [Column("until_date"), Required]
        public DateTime UntilDate { get; set; }
        [Column("is_active"), Required]
        public int IsActive { get; set; }
        [Column("percentage")]
        public Int16 Percentage { get; set; }
        [Column("times_factor")]
        public Int16 TimesFactor { get; set; }
        [Column("is_variable")]
        public bool IsVariable { get; set; }

        //[ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
