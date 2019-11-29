using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("PromotionGroup")]
   public class PromotionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_promotion_group")]
        public int Id { get; set; }
        [Column("description")]
        public string description { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

    }
}
