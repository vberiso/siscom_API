using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Requirement_For_Step")]
    public class RequirementForStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_note_procedure")]
        public int Id { get; set; }
        [Column("document_name")]
        public string DocumentName { get; set; }
        public int StepProcedureId { get; set; }
        public StepProcedure StepProcedure { get; set; }
    }
}
