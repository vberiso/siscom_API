using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Tax_Receipt")]
    public class TaxReceipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_tax_receipt")]
        public int Id { get; set; }
        [Column("tax_receipt_date"), Required]
        public DateTime TaxReceiptDate { get; set; }
        [Column("tax_receipt_xml")]
        public string XML { get; set; }
        [Column("tax_receipt_xml_fiel")]
        public string FielXML { get; set; }
        [Column("rfc"), StringLength(17)]
        public String RFC { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
