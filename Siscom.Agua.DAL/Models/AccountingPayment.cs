using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("accounting_payment")]
    public class AccountingPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_accounting_payment")]
        public int Id { get; set; }
        [Column("id_payment")]
        public int PaymentId { get; set; }
        [Column("code_sac")]
        public int CodeSAC { get; set; }
        [Column("procedure_code")]
        public int ProcedureCode { get; set; }
        [Column("secuential")]
        public int Secuential { get; set; }
        [Column("Amount")]
        public decimal Amount { get; set; }
        [Column("amount_tax")]
        public decimal AmountTax { get; set; }
        [Column("desciption_code"), StringLength(500)]
        public string DesciptionCode { get; set; }
        [Column("request_date")]
        public DateTime RequestDate { get; set; }
        [Column("is_dispatched")]
        public int IsDispatched { get; set; }
        [Column("movement_type"), StringLength(10)]
        public string MovementType { get; set; }
        [Column("accounting_account"), StringLength(20)]
        public string AccountingAccount { get; set; }
    }
}
