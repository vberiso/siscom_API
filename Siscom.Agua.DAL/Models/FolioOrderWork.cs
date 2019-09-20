using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Folio_order_work")]
    public class FolioOrderWork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_folio_order_work")]
        public int Id { get; set; }
        [Column("prefix"), StringLength(3)]
        public string Prefix { get; set; }
        [Column("secuential"),  Required]
        public int Secuential { get; set; }
        [Column("suffixes"), StringLength(3)]
        public string Suffixes { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
    }
}
