using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("debt_campaign_files")]
    public class DebtCampaignFiles
    {
        [Key]
        [Column("id_debt_campaign_files")]
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
        [Column("pdf")]
        public byte[] PDF { get; set; }

        [Column("total_records")]
        public int TotalRecords { get; set; }

        public bool IsInvitation { get; set; }

        [Column("folio")]
        public string Folio { get; set; }
    }
}
