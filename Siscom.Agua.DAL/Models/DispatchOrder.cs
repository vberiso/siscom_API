using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("DispatchOrder")]
    public class DispatchOrder
    {
        [Key]
        [Column("id_dispatch_order"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        [Column("orderworkid")]
        public int OrderWorkId { get; set; }

        [Column("technicalstaffid")]
        public int TechnicalStaffId { get; set; }

        [Column("IMEI"), StringLength(50)]
        public string IMEI { get; set; }

        [Column("date_asign")]
        public DateTime DateAsign { get; set; }
        [Column("date_attended")]
        public DateTime DateAttended { get; set; }
    }
}
