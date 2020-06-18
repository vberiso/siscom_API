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
        public OrderWork()
        {
            OrderWorkReasonCatalogs = new HashSet<OrderWorkReasonCatalog>();
            PhotosOrderWork = new HashSet<PhotosOrderWork>();
            OrderWorkStatus = new HashSet<OrderWorkStatus>();
            OrderWorkDetails = new HashSet<OrderWorkDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_work")]
        public int Id { get; set; }

        [Column("folio"), Required, StringLength(30)]
        public string Folio { get; set; }

        [Column("date_order"), Required]
        public DateTime DateOrder { get; set; }

        [Column("applicant"), Required, StringLength(150)]
        public string Applicant { get; set; }


        [Column("type"), Required, StringLength(6)]
        public string Type { get; set; }

        [Column("status"), Required, StringLength(6)]
        public string Status { get; set; }

        [Column("observation"), StringLength(800)]
        public string Observation { get; set; }

        [Column("date_stimated")]
        public DateTime DateStimated { get; set; }

        [Column("date_realization"), Required]
        public DateTime DateRealization { get; set; }

        [Column("activities"), Required, StringLength(250)]
        public string Activities { get; set; }

        [Column("AgreementId"), Required]
        public int AgrementId { get; set; }

        [Column("TaxUserId"), Required]
        public int TaxUserId { get; set; }

        [Column("TechnicalStaffId")]
        public int? TechnicalStaffId { get; set; }

        [Column("aviso")]
        public int? aviso { get; set; }

        [Column("DebtId")]
        public int? DebtId { get; set; }

        [Column("observation_mobile")]
        public string ObservationMobile { get; set; }

        //public Debt Debt { get; set; }
        public TechnicalStaff TechnicalStaff { get; set; }
        public Agreement Agreement { get; set; }
        public ICollection<OrderWorkStatus> OrderWorkStatus { get; set; }
        public ICollection<OrderWorkReasonCatalog> OrderWorkReasonCatalogs { get; set; }
        public ICollection<PhotosOrderWork> PhotosOrderWork { get; set; }
        public ICollection<LocationOrderWork> LocationOrderWorks { get; set; }
        public ICollection<OrderWorkDetail> OrderWorkDetails { get; set; }
        public ICollection<MaterialMovements> MaterialMovements { get; set; }
        public ICollection<OrderWorkList> OrderWorkLists { get; set; }

    }
}
