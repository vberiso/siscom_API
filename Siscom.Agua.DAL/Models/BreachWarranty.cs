using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Warranty")]
    public class BreachWarranty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_warranty")]
        public int Id { get; set; }
        [Column("references"), Required, StringLength(100)]
        public string References { get; set; }
        [Column("observations"), Required, StringLength(256)]
        public string Observations { get; set; }

        public int BreachId { get; set; }
        public Breach Breach { get; set; }

        public int WarrantyId { get; set; }
        public Warranty Warranty { get; set; }
    }
}
