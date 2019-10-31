using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("online_payment_file")]
    public class OnlinePaymentFile
    {
        [Key]
        [Column("online_payment_file_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("date_generated")]
        public DateTime DateGenerated { get; set; }
        [Column("id_agreement")]
        public int idAgreement { get; set; }
        [Column("account")]
        public string Account { get; set; }
        [Column("token")]
        public string Token { get; set; }
        [Column("folio")]
        public string Folio { get; set; }
        [Column("month")]
        public int Month { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("pdf_invoce")]
        public byte[] PDFInvoce { get; set; }

    }
}
