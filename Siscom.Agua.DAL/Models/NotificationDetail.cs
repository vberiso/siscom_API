using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Notification_Detail")]
    public class NotificationDetail
    {
        [Key]
        [Column("id_notification_detail"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("amount"), Required]
        public double Amount { get; set; }     
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Required, StringLength(5), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(150), Column("name_concept")]
        public string NameConcept { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
    }
}
