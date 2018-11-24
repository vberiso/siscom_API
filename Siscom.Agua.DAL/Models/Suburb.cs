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
        [Column("id_suburb"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(100), Required]
        public string Name { get; set; }

        public Town Towns { get; set; }
        public Region Regions { get; set; }
        public Clasification Clasifications { get; set; }

    }
}
