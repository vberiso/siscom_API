using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Country")]
    public class Country
    {
        public Country()
        {
            states = new HashSet<State>();
        }

        [Column("id_country"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(30), Required]
        public string Name { get; set; }
        [Column("abbreviation"), StringLength(3), Required]
        public string Abbreviation { get; set; }

        public ICollection<State> states { get; set; }
    }
}
