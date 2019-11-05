using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("account_status_In_File")]
    public class AccountStatusInFile
    {
        [Key]
        [Column("account_status_In_File_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("generation_date")]
        public DateTime GenerationDate { get; set; }
        [Column("file_name"), StringLength(200)]
        public string FileName { get; set; }
        [Column("folio")]
        public string Folio { get; set; }
        [Column("AgreementId")]
        public int AgreementId { get; set; }

        [Column("PDFBytes")]
        public byte[] PDFBytes { get; set; }
        public Agreement Agreement { get; set; }
    }
}
