using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
   [Table("Tariff")]
    public class Tariff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_tariff")]
        public int Id { get; set; }
        [Column("concept"), StringLength(80), Required]
        public string Concept { get; set; }
        [Column("account_number"), StringLength(20), Required]
        public string AccountNumber { get; set; }
        [Column("unit_measurement"), StringLength(10), Required]
        public string UnitMeasurement { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Column("percentage"), Required]
        public Int16 Percentage { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("from_date"), Required]
        public DateTime FromDate { get; set; }
        [Column("until_date"), Required]
        public DateTime UntilDate { get; set; }
        [Column("is_active"), Required]
        public int IsActive { get; set; }
        [Column("start_consume")]
        public decimal StartConsume { get; set; }
        [Column("end_consume")]
        public decimal EndConsume { get; set; }
        [Required, Column("have_consume")]
        public bool HaveConsume { get; set; }

        //[ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int TypeIntakeId { get; set; }
        public TypeIntake TypeIntake { get; set; }

        public int TypeConsumeId { get; set; }
        public TypeConsume TypeConsume { get; set; }
    }
}
