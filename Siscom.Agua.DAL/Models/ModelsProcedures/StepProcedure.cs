using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Step_Procedure")]
    public class StepProcedure
    {
        [Column("id_step_procedure"), Required]
        public int Id { get; set; }
        [Column("position"), Required]
        public int Position { get; set; }
        [Column("step_name"), Required]
        public string StepNamne { get; set; }
        [Column("recommended_days")]
        public int RecommendedDays { get; set; }
        [Column("PermitsStepId"), Required] //este va en la tabla de permit_step_procedure
        public int PermitsStepId { get; set; }
        [Column("AvailableProcedureId"), Required] 
        public int AvailableProcedureId { get; set; }
        public AvailableProcedure AvailableProcedure { get; set; }
    }
}
