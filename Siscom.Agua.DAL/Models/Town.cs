using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Town")]
    public class Town
    {
        public Town()
        {
            Suburbs = new HashSet<Suburb>();
        }

        [Column("id_town"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(30), Required]
        public string Name { get; set; }

        public int StateId { get; set; }
        public State States { get; set; }

        public ICollection<Suburb> Suburbs { get; set; }
    }
}
