using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Suburb")]
    public class Suburb
    {
        public Suburb()
        {
            Addresses = new HashSet<Address>();
        }

        [Column("id_suburb"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(100), Required]
        public string Name { get; set; }

        //[ForeignKey("Towns")]
        public int TownsId { get; set; }
        public Town Towns { get; set; }

        //[ForeignKey("Regions")]
        public int RegionsId { get; set; }
        public Region Regions { get; set; }

        //[ForeignKey("Clasifications")]
        public int ClasificationsId { get; set; }
        public Clasification Clasifications { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
