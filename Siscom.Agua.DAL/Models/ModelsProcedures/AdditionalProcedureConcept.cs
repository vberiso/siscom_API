using Siscom.Agua.DAL.Models.ModelsProcedures;
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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_additional_procedure_concept"), Required]
        public int Id { get; set; }
        [Column("description"), Required, StringLength(255)]
        public string Description { get; set; }
        [Column("cost"), Required]
        public decimal Cost { get; set; }


        public int AvailableProcedureId { get; set; }
        public AvailableProcedure AvailableProcedure { get; set; }
    }
}
