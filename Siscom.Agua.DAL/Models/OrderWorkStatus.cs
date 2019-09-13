using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_work_Status")]
   public class OrderWorkStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_work_status")]
        public int Id { get; set; }

        [Column("id_status"), Required, StringLength(10)]
        public string IdStatus { get; set; }

        [Column("order_work_status_date"), Required]
        public DateTime OrderWorkStatusDate { get; set; }

        [Column("user"), Required, StringLength(80)]
        public string User { get; set; }

        [Column("OrderWorkId")]
        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }
    }
}
