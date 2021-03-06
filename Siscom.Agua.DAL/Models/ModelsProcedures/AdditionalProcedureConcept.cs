using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Additional_Procedure_Concept")]
    public class AdditionalProcedureConcept
    {
        [Column("id_additional_procedure_concept"), Required]
        public int Id { get; set; }
        [Column("description"), Required]
        public string Description { get; set; }
        [Column("cost"), Required]
        public decimal Cost { get; set; }
        [Column("AvailableProcedureId"), Required]
        public int AvailableProcedureId { get; set; }
    }
}
