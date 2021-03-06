using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Permit_Step_Procedure")]
    public class PermitStepProcedure
    {
        [Column("id_permit_step_procedure"), Required]
        public int Id { get; set; }
        [Column("can_date"), Required]
        public bool CanDate { get; set; }
        [Column("can_order"), Required]
        public bool CanOrder { get; set; }
        [Column("can_document"), Required]
        public bool CanDocument { get; set; }
        [Column("can_note"), Required]
        public bool CanNote { get; set; }
    }
}
