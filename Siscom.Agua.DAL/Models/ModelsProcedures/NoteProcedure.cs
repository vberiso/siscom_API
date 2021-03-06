using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    class NoteProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_note_procedure")]
        public int Id { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime CreateDate { get; set; }
        

        //no estoy seguro de que este vaya aqui
        public int DivisionId { get; set; }
        public Division Division { get; set; }


        [Column("UserId")]
        public String UserId { get; set; }


        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }
    }
}
