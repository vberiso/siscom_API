using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
   public class DebtCampaign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_debt_campaign")]
        public int Id { get; set; }

        [Column("ruta")]
        public int Ruta { get; set; }

        [Column("AgreementId")]
        public int AgreementId { get; set; }

        [Column("account"), StringLength(20)]
        public string Account { get; set; }

        [Column("start_year_debt"), StringLength(20)]
        public string StartYearDebt { get; set; }

        [Column("end_year_debt"), StringLength(20)]
        public string EndYearDebt { get; set; }

        [Column("importe")]
        public decimal Importe { get; set; }

        [Column("iva")]
        public decimal Iva { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("total_agua")]
        public decimal TotalAgua { get; set; }

        [Column("total_drenaje")]
        public decimal TotalDrenaje { get; set; }

        [Column("total_saneamiento")]
        public decimal TotalSaneamiento { get; set; }

        [Column("status"), StringLength(10)]
        public string Status { get; set; }

        [Column("folio"), StringLength(10)]
        public string Folio { get; set; }

        [Column("date_subscription")]
        public DateTime DateSubscription { get; set; }

        [Column("DebtId"), StringLength(15)]
        public string DebtId { get; set; }

        [Column("servicios"), StringLength(100)]
        public string Servicios { get; set; }
        [Column("consumo"), StringLength(60)]
        public string Consumo { get; set; }
        [Column("importe_multas")]
        public decimal ImporteMultas { get; set; }
        [Column("importe_recargo")]
        public decimal ImporteRecargo { get; set; }
        [Column("importe_notificaciones")]
        public decimal ImporteNotificaciones { get; set; }
        [Column("descuento_multa")]
        public decimal DescuentoMulta { get; set; }
        [Column("descuento_recargo")]
        public decimal DescuentoRecargo { get; set; }
        [Column("descuento_notificaciones")]
        public decimal DescuentoNotificaciones { get; set; }
        [Column("total_descuento_servicios")]
        public decimal TaotalDescuentoServicios { get; set; }
        public Agreement Agreement { get; set; }
    }
}
