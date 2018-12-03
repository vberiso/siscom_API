using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Clasification")]
    public class Clasification
    {
        public Clasification()
        {
            Suburbs = new HashSet<Suburb>();
        }
        
        [Column("id_clasification"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }

        public ICollection<Suburb> Suburbs { get; set; }

    }
}
