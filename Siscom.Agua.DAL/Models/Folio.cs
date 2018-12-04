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
        [Column("range"), Required]
        public string Range { get; set; }
        [Column("initial"), Required]
        public int Initial { get; set; }
        [Column("secuential"), Required]
        public int Secuential { get; set; }
        [Column("date_current"), Required]
        public DateTime DateCurrent { get; set; }
        [Column("is_active"), Required]
        public int IsActive { get; set; }

        //[ForeignKey("BranchOffice")]
        public int BranchOfficeId { get; set; }
        public BranchOffice BranchOffice { get; set; }
    }
}
