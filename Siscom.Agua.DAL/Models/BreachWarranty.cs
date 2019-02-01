using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Warranty")]
    public class BreachWarranty
    {
        public BreachWarranty()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_Warranty")]
        public int Id { get; set; }

        [Column("references"), Required, StringLength(100)]
        public string References { get; set; }

        [Column("observations"), Required, StringLength(100)]
        public string Observations { get; set; }


        public int BreachId { get; set; }
        public Breach Breach { get; set; }

        public int WarrantyId { get; set; }
        public Warranty Warranty { get; set; }









    }
}
