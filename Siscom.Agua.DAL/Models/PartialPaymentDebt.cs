using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Partial_Payment_Debt")]
    public class PartialPaymentDebt
    {
        [Key]
        [Column("id_partial_payment_debt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("DebtId")]
        public int DebtId { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("on_account")]
        public decimal OnAccount { get; set; }
        [Column("status"), StringLength(5)]
        public string Status { get; set; }
        public int PartialPaymentId { get; set; }
        public PartialPayment PartialPayment { get; set; }
    }
}
