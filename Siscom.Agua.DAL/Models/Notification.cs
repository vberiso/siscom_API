using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Notification")]
    public class Notification
    {
        public Notification()
        {
            NotificationDetails = new HashSet<NotificationDetail>();
        }

        [Key]
        [Column("id_notification"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("folio"), StringLength(40), Required]
        public string Folio { get; set; }
        [Column("notification_date"), Required]
        public DateTime NotificationDate { get; set; }
        [Column("from_date"), Required]
        public DateTime FromDate { get; set; }
        [Column("until_date"), Required]
        public DateTime UntilDate { get; set; }      
        [Column("subtotal"), Required]
        public decimal Subtotal { get; set; }      
        [Column("tax"), Required]
        public decimal Tax { get; set; }
        [Column("rounding"), Required]
        public decimal Rounding { get; set; }
        [Column("total"), Required]
        public decimal Total { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public ICollection<NotificationDetail> NotificationDetails { get; set; }
    }
}
