using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Note_Procedure")]
    public class NoteProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_note_procedure")]
        public int Id { get; set; }
        [Column("title"), Required, StringLength(170)]
        public String Title { get; set; }
        [Column("description"), StringLength(500)]
        public String Description { get; set; }
        [Column("create_date"), Required]
        public DateTime CreateDate { get; set; }
        //no estoy seguro de que este vaya aqui
        [Required]
        public int DivisionId { get; set; }

        [Required]
        public String UserId { get; set; }


        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }
    }
}
