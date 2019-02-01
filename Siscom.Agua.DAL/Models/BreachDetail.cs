using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Detail")]
    public class BreachDetail
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_detail")]
        public int Id { get; set; }       
        [Required, Column("aplication_days")]
        public int AplicationDays { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("porcent_bonification"), Required]
        public decimal PercentBonification { get; set; }
        [Column("bonification"), Required]
        public decimal Bonification { get; set; }

        public int BreachId { get; set; }
        public Breach  Breach { get; set; }

        public int BreachListId { get; set; }
        public BreachList BreachList { get; set; }
    }
}
