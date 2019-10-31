using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{

    [Table("partial_payment")]
    public class PartialPayment
    {
        public PartialPayment()
        {
            PartialPaymentDetails = new HashSet<PartialPaymentDetail>();
            PartialPaymentDebts = new HashSet<PartialPaymentDebt>();
        }

        [Key]
        [Column("id_partial_payment")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("folio"), StringLength(30)]
        public string Folio { get; set; }
        [Column("partial_payment_date")]
        public DateTime PartialPaymentDate { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("number_of_payments")]
        public int NumberOfPayments { get; set; }
        [Column("initial_payment")]
        public decimal InitialPayment { get; set; }
        [Column("status"), StringLength(5)]
        public string Status { get; set; }
        [Column("type_intake"), StringLength(50)]
        public string TypeIntake { get; set; }
        [Column("type_service"), StringLength(50)]
        public string TypeService { get; set; }
        [Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }
        [Column("from_date")]
        public DateTime FromDate { get; set; }
        [Column("until_date")]
        public DateTime UntilDate { get; set; }
        [Column("observations"), StringLength(1000)]
        public string Observations { get; set; }
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
        [Column("signature_name"), StringLength(200)]
        public string SignatureName { get; set; }
        [Column("identification_card"), StringLength(200)]
        public string IdentificationCard { get; set; }
        [Column("identification_number"), StringLength(200)]
        public string IdentificationNumber { get; set; }
        [Column("email"), StringLength(200)]
        public string Email { get; set; }
        [Column("phone"), StringLength(200)]
        public string Phone { get; set; }
        public ICollection<PartialPaymentDetail> PartialPaymentDetails { get; set; }
        public ICollection<PartialPaymentDebt> PartialPaymentDebts { get; set; }
    }
}
