using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Diameter")]
    public class Diameter
    {
        public Diameter()
        {
            Agreements = new HashSet<Agreement>();
        }

        [Key]
        [Column("id_diameter"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }
        [Required, StringLength(5), Column("name")]
        public string Name { get; set; }
        [Required, StringLength(20), Column("description")]
        public string Description { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
