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
        public StepProcedure()
        {
            RequirementForSteps = new HashSet<RequirementForStep>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_step_procedure"), Required]
        public int Id { get; set; }
        [Column("position"), Required]
        public int Position { get; set; }
        [Column("step_name"), Required, StringLength(255)]
        public string StepName { get; set; }
        [Column("recommended_days")]
        public int RecommendedDays { get; set; }
        [Column("can_date"), Required]
        public bool CanDate { get; set; }
        [Column("can_order"), Required]
        public bool CanOrder { get; set; }
        [Column("can_document"), Required]
        public bool CanDocument { get; set; }
        [Column("can_note"), Required]
        public bool CanNote { get; set; }


        public int AvailableProcedureId { get; set; }
        public AvailableProcedure AvailableProcedure { get; set; }


        public ICollection<RequirementForStep> RequirementForSteps { get; set; }
    }
}
