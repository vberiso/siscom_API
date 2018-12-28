using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Push_Notification")]
    public class PushNotifications
    {
        [Key, Column("id_notification"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("type"), Required]
        public string Type { get; set; }
        [Column("agreement_id")]
        public int AgreementId { get; set; }
        [Column("debt_id")]
        public int DebtId { get; set; }
        [Column("folio"), StringLength(40)]
        public string Folio { get; set; }
        [Column("porcentage")]
        public byte Porcentage { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("reason"), Required]
        public string Reason { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
