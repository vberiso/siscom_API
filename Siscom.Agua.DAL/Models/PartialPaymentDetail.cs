using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("partial_payment_detail")]
    public class PartialPaymentDetail
    {
        public PartialPaymentDetail()
        {
            PartialPaymentDetailStatuses = new HashSet<PartialPaymentDetailStatus>();
            PartialPaymentDetailConcepts = new HashSet<PartialPaymentDetailConcept>();
        }

        [Key]
        [Column("id_partial_payment_detail")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("payment_number")]
        public int PaymentNumber { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("on_account")]
        public decimal OnAccount { get; set; }
        [Column("status"), StringLength(5)]
        public string Status { get; set; }
        [Column("relase_date")]
        public DateTime RelaseDate { get; set; }
        [Column("relase_debtId")]
        public int RelaseDebtId { get; set; }
        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }
        public int PaymentId { get; set; }
        public int PartialPaymentId { get; set; }
        public PartialPayment PartialPayment { get; set; }
        public ICollection<PartialPaymentDetailStatus> PartialPaymentDetailStatuses { get; set; }
        public ICollection<PartialPaymentDetailConcept> PartialPaymentDetailConcepts { get; set; }
    }
}
