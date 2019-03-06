using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Detail")]
    public class BreachDetail
    {
        [Column("id_breach"), Required]
        public int BreachId { get; set; }
        public Breach Breach { get; set; }

        [Column("id_breach_list"), Required]
        public int BreachListId { get; set; }
        public BreachList BreachList { get; set; }
        
        [Column("times_factor"), Required]
        public Int16 TimesFactor { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("porcent_bonification"), Required]
        public decimal PercentBonification { get; set; }
        [Column("bonification"), Required]
        public decimal Bonification { get; set; }       
    }
}
