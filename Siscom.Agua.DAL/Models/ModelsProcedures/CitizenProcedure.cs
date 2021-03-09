using Siscom.Agua.DAL.Models.ModelsProcedures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Citizen_Procedure")]
    public class CitizenProcedure
    {
        public CitizenProcedure()
        {
            DateProcedures = new HashSet<DateProcedure>();
            DocumentProcedures = new HashSet<DocumentProcedure>();
            NoteProcedures = new HashSet<NoteProcedure>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_citizen_procedure"), Required]
        public int Id { get; set; }
        [Column("folio"), Required]
        public string Folio { get; set; }
        [Column("begin_date"), Required]
        public DateTime BeginDate { get; set; }
        [Column("finish_date")]
        public DateTime FinishDate { get; set; }
        [Column("closing_remark"), StringLength(500)]
        public string ClosingRemark { get; set; }
        [Column("last_update_date")]
        public DateTime LastUpdateDate { get; set; }
        [Column("current_step"), Required]
        public int CurrentStep { get; set; }
        [Column("status"), Required]
        public string Status { get; set; }
        [Column("meet_all_requirements"), Required]
        public bool MeetAllRequirements { get; set; }
        [Column("AvailableProcedureId"), Required]


        public int AvailableProcedureId { get; set; }
        public AvailableProcedure AvailableProcedure { get; set; }

        public ICollection<DateProcedure> DateProcedures { get; set; }
        public ICollection<DocumentProcedure> DocumentProcedures { get; set; }
        public ICollection<NoteProcedure> NoteProcedures { get; set; }

    }
}
