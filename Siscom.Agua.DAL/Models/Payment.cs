using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_payment")]
        public int Id { get; set; }
        [Column("payment_date"), Required]
        public DateTime PaymentDate { get; set; }
        [Column("branch_office"), StringLength(20), Required]
        public string BranchOffice { get; set; }
        [Column("subtotal"), Required]
        public double Subtotal { get; set; }
        [Column("percentage_tax"), StringLength(2)]
        public string PercentageTax { get; set; }
        [Column("tax"), Required]
        public double Tax { get; set; }
        [Column("total"), Required]
        public double Total { get; set; }
        [Column("authorization"), StringLength(50)]
        public string Authorization { get; set; }

        [Column("debt"), Required]
        public int Debt { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }

        public OriginPayment OriginPayment { get; set; }
        public PayMethod PayMethod { get; set; }
    }
}
