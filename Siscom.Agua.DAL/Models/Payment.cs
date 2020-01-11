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
            DetailOfPaymentMethods = new HashSet<DetailOfPaymentMethods>();
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
        [Column("number_bank"), StringLength(33)]
        public string NumberBank { get; set; }
        [Column("account_number"), StringLength(25)]
        public string AccountNumber { get; set; }
        [Column("transaction_folio"), StringLength(40)]
        public string TransactionFolio { get; set; }
        [Column("id_agreement"), Required]
        public int AgreementId { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Column("pay_method_number"), StringLength(31)]
        public string PayMethodNumber { get; set; }
        [Column("have_tax_receipt"), Required]
        public bool HaveTaxReceipt { get; set; }
        [Column("account"), StringLength(50)]
        public string Account { get; set; }
        [Column("id_order_sale"), Required]
        public int OrderSaleId { get; set; }
        [Column("ObservationInvoice"), StringLength(600)]
        public string ObservationInvoice { get; set; }
        [Column("TerminalUserId")]
        public int TerminalUserId { get; set; }
        [Column("ImpressionSheet")]
        public string ImpressionSheet { get; set; }

        //[ForeignKey("OriginPayment")]
        public int OriginPaymentId { get; set; }
        public OriginPayment OriginPayment { get; set; }

        //[ForeignKey("ExternalOriginPayment")]
        public int ExternalOriginPaymentId { get; set; }
        public ExternalOriginPayment ExternalOriginPayment { get; set; }

        //[ForeignKey("PayMethod")]
        public int PayMethodId { get; set; }
        public PayMethod PayMethod { get; set; }

        [Column("cash_payment")]
        public decimal CashPayment { get; set; }
        [Column("caed_payment")]
        public decimal CardPayment { get; set; }
        [Column("bank_draft_payment")]
        public decimal BankDraftPayment { get; set; }
        [Column("transference_payment")]
        public decimal TansferencePayment { get; set; }
        public ICollection<PaymentDetail> PaymentDetails { get; set; }
        public ICollection<TaxReceipt> TaxReceipts { get; set; }
        public ICollection<DetailOfPaymentMethods> DetailOfPaymentMethods { get; set; }
    }
}
