﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt")]
    public class Debt
    {
        public Debt()
        {
            DebtDetails = new HashSet<DebtDetail>();
            DebtStatuses = new HashSet<DebtStatus>();
            DebtDiscounts = new HashSet<DebtDiscount>();
        }

        [Key]
        [Column("id_debt"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public decimal Amount { get; set; }
        [Column("on_account"), Required]
        public decimal OnAccount { get; set; }
        [NotMapped]
        [Column("on_payment"), Required]
        public decimal OnPayment { get; set; }
        [Column("year"), Required]
        public Int16 Year { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("DebtPeriodId")]
        public int? DebtPeriodId { get; set; }
        [Column("expiration_date"), Required]
        public DateTime ExpirationDate { get; set; }

        //[ForeignKey("Agreement")]
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public ICollection<DebtDetail> DebtDetails { get; set; }
        public ICollection<DebtStatus> DebtStatuses { get; set; }
        public ICollection<DebtDiscount> DebtDiscounts { get; set; }
    }
}
