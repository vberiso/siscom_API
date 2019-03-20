using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Translation_Code")]
    public class TranslationCode
    {
        [Key]
        [Column("id_product", Order = 0)]
        public int ProductId { get; set; }
        [Key]
        [Column("id_group_translation", Order = 1)]
        public int GroupTranslationCodeId { get; set; }

        [Column("code"), StringLength(30), Required]
        public string Code { get; set; }

        public GroupTranslationCode GroupTranslationCode { get; set; }
    }
}
