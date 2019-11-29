using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("PromotionDebt")]
   public class PromotionDebt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_promotion_debt")]
        public int Id { get; set; }
        [Column("PromotionId")]
        public int PromotionId { get; set; }


        [Column("DebtId")]
        public int DebtId { get; set; }

        [Column("use")]
        public string user { get; set; }


        [Column("use_id")]
        public string userId { get; set; }

        [Column("DebtApplyPromotion")]
        public DateTime DebtApplyPromotion { get; set; }
    }
}
