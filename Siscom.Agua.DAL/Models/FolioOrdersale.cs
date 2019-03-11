using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("FolioOrdersale")]
    public class FolioOrdersale
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_folio_breach")]
        public int Id { get; set; }
        [Column("prefix")]
        public string Prefix { get; set; }
        [Column("secuential"), Required]
        public int Secuential { get; set; }
        [Column("suffixes")]
        public string Suffixes { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        [Column("type")]
        public string Type { get; set; }

    }
}
