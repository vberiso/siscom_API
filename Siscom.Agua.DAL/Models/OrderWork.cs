using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_work")]
    public class OrderWork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_work")]
        public int Id { get; set; }

        [Column("folio"), Required]
        public string Folio { get; set; }

        [Column("date_order"), Required]
        public DateTime DateOrder { get; set; }

        [Column("applicant"), Required]
        public string Applicant { get; set; }


        [Column("type"), Required]
        public string Type { get; set; }

        [Column("status"), Required]
        public string Status { get; set; }

        [Column("observation")]
        public string Observation { get; set; }

        [Column("date_stimated")]
        public DateTime DateStimated { get; set; }

        [Column("date_realization"), Required]
        public DateTime DateRealization { get; set; }

        [Column("activities"), Required]
        public string Activities { get; set; }

        [Column("AgreementId"), Required]
        public int AgrementId { get; set; }

        [Column("TaxUserId"), Required]
        public int TaxUserId { get; set; }

        [Column("TechnicalStaffId"), Required]
        public int TechnicalStaffId { get; set; }


        public ICollection<OrderWork> OrderWorkStatus { get; set; }





    }
}
