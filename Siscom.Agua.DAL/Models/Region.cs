using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Region")]
    public class Region
    {
        public Region()
        {
            Suburbs = new HashSet<Suburb>();
        }

        [Column("id_region"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), Required]
        public int Name { get; set; }

        public ICollection<Suburb> Suburbs { get; set; }
    }
}
