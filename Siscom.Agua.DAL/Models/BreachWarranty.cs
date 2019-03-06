using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Warranty")]
    public class BreachWarranty
    {       
        [Column("id_breach"), Required]
        public int BreachId { get; set; }
        public Breach Breach { get; set; }

        [Column("id_warranty"), Required]
        public int WarrantyId { get; set; }
        public Warranty Warranty { get; set; }

        [Column("references"), Required, StringLength(100)]
        public string References { get; set; }
        [Column("observations"), Required, StringLength(256)]
        public string Observations { get; set; }
    }
}
