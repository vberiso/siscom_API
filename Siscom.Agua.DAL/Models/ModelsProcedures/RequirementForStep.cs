using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    class RequirementForStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_note_procedure")]
        public int Id { get; set; }

        [Column("id_note_procedure")]
        public String DocumentName { get; set; }

        public int StepProcedureId { get; set; }
        public StepProcedure StepProcedure { get; set; }
    }
}
