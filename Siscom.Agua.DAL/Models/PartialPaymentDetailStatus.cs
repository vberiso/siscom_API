using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("partial_payment_detail_status")]
    public class PartialPaymentDetailStatus
    {
        [Key]
        [Column("id_partial_payment_detail_status")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("status"), StringLength(5)]
        public string Status { get; set; }
        [Column("partial_payment_detail_status_date")]
        public DateTime PartialPaymentDetailStatusDate { get; set; }
        [Column("user"), StringLength(80)]
        public string User { get; set; }
        public int PartialPaymentDetailId { get; set; }
        public PartialPaymentDetail PartialPaymentDetail { get; set; }
    }
}
