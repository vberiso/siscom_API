using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    class DateProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_date_procedure")]
        public int Id { get; set; }

        [Column("createDate")]
        public DateTime CeateDate { get; set; }

        [Column("beginDate")]
        public DateTime BeginDate { get; set; }

        [Column("finishDate")]
        public DateTime FinishDate { get; set; }

        [Column("reason")]
        public String Reason { get; set; }

        [Column("place")]
        public String Place { get; set; }

        [Column("done")]
        public Boolean Done { get; set; }

        [Column("UserId")]
        public String UserId{ get; set; }


        //no estoy seguro de que este vaya aqui
        public int DivisionId { get; set; }
        public Division Division { get; set; }


        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }


    }
}
