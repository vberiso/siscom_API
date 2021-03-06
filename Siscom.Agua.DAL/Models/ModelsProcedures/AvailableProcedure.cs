using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Available_Procedure")]
    public class AvailableProcedure
    {
        [Column("id_available_procedure"), Required]
        public int Id { get; set; }
        [Column("procedure_description"), Required]
        public string ProcedureDescription { get; set; }
        [Column("cost"), Required]
        public decimal Cost { get; set; }
        [Column("isActive"), Required]
        public bool IsActive { get; set; }
        [Column("DivisionId"), Required]
        public int DivisionId { get; set; }
        public Division Division { get; set; }
    }
}
