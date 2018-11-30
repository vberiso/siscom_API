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
        [Column("amount"), Required]
        public double Amount { get; set; }
        [Column("on_account"), Required]
        public double OnAccount { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("Debt")]
        public int DebtId { get; set; }
        public Debt Debt { get; set; }

    }
}
