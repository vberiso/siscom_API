using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Division")]
    public class Division
    {
        public Division()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_division")]
        public int Id { get; set; }
        [Required, StringLength(150), Column("name")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
