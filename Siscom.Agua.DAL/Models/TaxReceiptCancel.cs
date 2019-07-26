using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Tax_Receipt_Cancel")]
    public class TaxReceiptCancel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_tax_receipt_cancel")]
        public int Id { get; set; }
        [Column("status"), StringLength(100)]
        public string Status { get; set; }
        [Column("message"), StringLength(200)]
        public string Message { get; set; }
        [Column("requestDateCancel")]
        public DateTime RequestDateCancel { get; set; }
        [Column("cancelationDate")]
        public DateTime CancelationDate { get; set; }
        [Column("acuseXml")]
        public byte[] AcuseXml { get; set; }
        public int TaxReceiptId { get; set; }
        public TaxReceipt TaxReceipt { get; set; }
    }
}
