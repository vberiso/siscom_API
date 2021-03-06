using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("notification_files")]
    public class NotificationFiles
    {
        [Key]
        [Column("id_Notification_files")]
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
        [Column("pdf_notifications")]
        public byte[] PDFNotifications { get; set; }

        [Column("type_file")]
        public string TypeFile { get; set; }

        [Column("folio")]
        public string Folio { get; set; }

        [Column("total_records")]
        public int TotalRecords { get; set; }
    }
}
