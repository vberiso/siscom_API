using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Group_Translation_Code")]
    public class GroupTranslationCode
    {
        public GroupTranslationCode()
        {
            TranslationCodes = new HashSet<TranslationCode>();
        }

        [Column("id_group_translation_codes"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }

        public ICollection<TranslationCode> TranslationCodes { get; set; }
    }
}
