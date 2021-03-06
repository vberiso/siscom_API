using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("generic_folios")]
    public class GenericFolios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_generic_folio")]
        public int Id { get; set; }
        [Column("prefix"), StringLength(3)]
        public string Prefix { get; set; }
        [Column("secuential"), Required]
        public int Secuential { get; set; }
        [Column("suffixes"), StringLength(3)]
        public string Suffixes { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
    }
}
