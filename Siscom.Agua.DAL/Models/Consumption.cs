using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Consumption")]
    public class Consumption
    {
        [Key]
        [Column("id_consumption"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("consumption_date"), Required]
        public DateTime ConsumptionDate { get; set; }
        [Column("previous_consumption"), Required]
        public decimal PreviousConsumption { get; set; }
        [Column("current_consumption"), Required]
        public decimal CurrentConsumption { get; set; }
        [Column("consumption"), Required]
        public decimal consumption { get; set; }
        [Column("is_active"), Required]
        public bool is_active { get; set; }

        [Column("DebtId")]
        public int DebtId { get; set; }
        [Column("in_debt"), Required]
        public bool InDebt { get; set; }

        public int MeterId { get; set; }
        public Meter Meter { get; set; }
    }
}
