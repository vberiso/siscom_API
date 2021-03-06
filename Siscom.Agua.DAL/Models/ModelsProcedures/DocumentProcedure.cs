using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    class DocumentProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_document_procedure")]
        public int Id { get; set; }


        [Column("uploadDate")]
        public DateTime UploadDate { get; set; }

        [Column("sha512")]
        public String Sha512 { get; set; }

        [Column("UserId")]
        public String UserId { get; set; }

        //no estoy seguro de que este vaya aqui
        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }
    }
}
