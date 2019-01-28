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
        public Payment()
        {
            PaymentDetails = new HashSet<PaymentDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_payment")]
        public int Id { get; set; }
        [Column("payment_date"), Required]
        public DateTime PaymentDate { get; set; }
        [Column("branch_office"), StringLength(30), Required]
        public string BranchOffice { get; set; }
        [Column("subtotal"), Required]
        public decimal Subtotal { get; set; }
        [Column("percentage_tax"), StringLength(2)]
        public string PercentageTax { get; set; }
        [Column("tax"), Required]
        public decimal Tax { get; set; }
        [Column("rounding"), Required]
        public decimal Rounding { get; set; }
        [Column("total"), Required]
        public decimal Total { get; set; }
        [Column("authorization_origin_payment"), StringLength(50)]
        public string AuthorizationOriginPayment { get; set; }        
        [Column("transaction_folio"), StringLength(40)]
        public string TransactionFolio { get; set; }

        [Column("id_agreement"), Required]
        public int AgreementId { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }

        //[ForeignKey("OriginPayment")]
        public int OriginPaymentId { get; set; }
        public OriginPayment OriginPayment { get; set; }

        //[ForeignKey("ExternalOriginPayment")]
        public int ExternalOriginPaymentId { get; set; }
        public ExternalOriginPayment ExternalOriginPayment { get; set; }

        //[ForeignKey("PayMethod")]
        public int PayMethodId { get; set; }
        public PayMethod PayMethod { get; set; }

        public ICollection<PaymentDetail> PaymentDetails { get; set; }
    }
}
