using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Warranty")]
    public class Warranty
    {
        public Warranty()
        {
            BreachWarranty = new HashSet<BreachWarranty>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_Warranty")]
        public int Id { get; set; }

        [Column("description"), Required, StringLength(100)]
        public string Description { get; set; }

        public ICollection<BreachWarranty> BreachWarranty { get; set; }



    }
}
