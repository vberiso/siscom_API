using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Detail")]
    public class DebtDetail
    {
        [Key]
        [Column("id_debt_detail"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("on_account"), Required]
        public decimal OnAccount { get; set; }
        [NotMapped]
        public decimal OnPayment { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Required, StringLength(5), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(500), Column("name_concept")]
        public string NameConcept { get; set; }
        [Required,Column("quantity")]
        public decimal Quantity { get; set; }
        [NotMapped]
        [Column("tax"), Required]
        public decimal Tax { get; set; }
        [Column("old_value")]
        public decimal OldValue { get; set; }
        [Column("number_period")]
        public int NumberPeriod { get; set; }
        //[ForeignKey("Debt")]
        public int DebtId { get; set; }
        public Debt Debt { get; set; }

    }
}
