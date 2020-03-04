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

        [Column("account")]
        public string Account { get; set; }

        [Column("start_year_debt")]
        public int StartYearDebt { get; set; }

        [Column("end_year_debt")]
        public int EndYearDebt { get; set; }

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

        [Column("status")]
        public string Status { get; set; }

        [Column("folio")]
        public string Folio { get; set; }

        [Column("date_subscription")]
        public DateTime DateSubscription { get; set; }

        [Column("DebtId")]
        public string DebtId { get; set; }

        [Column("servicios")]
        public string Servicios { get; set; }
        [Column("consumo")]
        public string Consumo { get; set; }

        public Agreement Agreement { get; set; }
    }
}
