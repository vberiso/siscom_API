using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Calculation")]
    public class DebtCalculation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_debt_calculation")]
        public int Id { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("percentage")]
        public Int16 Percentage { get; set; }
        [Column("factor")]
        public Int16 Factor { get; set; }
        [Column("times_factor")]
        public Int16 TimesFactor { get; set; }
        [Column("is_variable")]
        public bool IsVariable { get; set; }

        public int DebtId { get; set; }
        public Debt Debt { get; set; }
    }
}
