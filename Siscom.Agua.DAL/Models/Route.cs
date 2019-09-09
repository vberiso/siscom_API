using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Route")]
    public class Route
    {
        [Key]
        [Column("id_route"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), Required]
        public string Name { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
    }
}
