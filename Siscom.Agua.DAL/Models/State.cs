using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("State")]
    public class State
    {
        public State()
        {
            Towns = new HashSet<Town>();
        }

        [Column("id_state"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(30), Required]
        public string Name { get; set; }

        public int CountriesId { get; set; }
        public Country Countries { get; set; }

        public ICollection<Town> Towns { get; set; }
    }
}
