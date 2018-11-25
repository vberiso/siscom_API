using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt")]
    public class Debt
    {
        [Key]
        [Column("id_debt"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Column("debit_date"), Required]
        public DateTime DebitDate { get; set; }
        [Column("from_date"), Required]
        public DateTime FromDate { get; set; }
        [Column("until_date"), Required]
        public DateTime UntilDate { get; set; }
        [Column("derivatives"), Required]
        public int Derivatives { get; set; }
        [Column("type_intake"), StringLength(50), Required]
        public string TypeIntake { get; set; }
        [Column("type_service"), StringLength(50), Required]
        public string TypeService { get; set; }
        [Column("consumption"), StringLength(10), Required]
        public string Consumption { get; set; }
        [Column("discount"), StringLength(50)]
        public string Discount { get; set; }
        [Column("amount"), Required]
        public double Amount { get; set; }
        [Column("on_account"), Required]
        public double OnAccount { get; set; }
        [Column("year"), Required]
        public Int16 Year { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        public DebtPeriod DebtPeriod { get; set; }
        public Agreement Agreement { get; set; }
    }
}
