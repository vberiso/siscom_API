using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Period")]
    public class DebtPeriod
    {
        public DebtPeriod()
        {
            Debts = new HashSet<Debt>();
        }

        [Key]
        [Column("id_debt_period"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]       
        public int Id { get; set; }
        [Column("period"), Required]
        public Int16 Period { get; set; }
        [Column("start_date"), Required]
        public DateTime StartDate { get; set; }
        [Column("end_date"), Required]
        public DateTime EndDate { get; set; }
        [Column("run_date"), Required]
        public DateTime RunDate { get; set; }
        [Column("run_hour"), Required]
        public TimeSpan RunHour { get; set; }

        public TypePeriod TypePeriod { get; set; }

        public ICollection<Debt> Debts { get; set; }
    }
}
