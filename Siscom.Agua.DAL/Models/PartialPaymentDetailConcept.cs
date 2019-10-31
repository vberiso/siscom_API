using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Partial_Payment_Detail_Concept")]
    public class PartialPaymentDetailConcept
    {
        [Key]
        [Column("id_partial_payment_detail_concept")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("on_account")]
        public decimal OnAccount { get; set; }
        [Column("have_tax")]
        public bool HaveTax { get; set; }
        [Column("code_concept"), StringLength(5)]
        public string CodeConcept { get; set; }
        [Column("name_concept"), StringLength(500)]
        public string NameConcept { get; set; }
        public int PartialPaymentDetailId { get; set; }
        public PartialPaymentDetail PartialPaymentDetail { get; set; }
    }
}
