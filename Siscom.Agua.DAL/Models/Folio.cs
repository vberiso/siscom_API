using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Folio")]
    public class Folio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_folio")]
        public int Id { get; set; }
        [Column("initial"), Required]
        public int Initial { get; set; }
        [Column("secuential"), Required]
        public int Secuential { get; set; }

        public BranchOffice BranchOffice { get; set; }
    }
}
