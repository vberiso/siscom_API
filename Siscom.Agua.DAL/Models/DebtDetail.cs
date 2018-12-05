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
        public double Amount { get; set; }
        [Column("on_account"), Required]
        public double OnAccount { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Required, StringLength(5), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(50), Column("name_concept")]
        public string NameConcept { get; set; }

        //[ForeignKey("Debt")]
        public int DebtId { get; set; }
        public Debt Debt { get; set; }

    }
}
