using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Date_Procedure")]
    public class DateProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_date_procedure"), Required]
        public int Id { get; set; }

        [Column("create_date"), Required]
        public DateTime CreateDate { get; set; }

        [Column("begin_date"), Required]
        public DateTime BeginDate { get; set; }

        [Column("finish_date"), Required]
        public DateTime FinishDate { get; set; }

        [Column("reason"), Required]
        public String Reason { get; set; }

        [Column("place"), Required]
        public String Place { get; set; }

        [Column("done"), Required]
        public Boolean Done { get; set; }

        [Required]
        public String UserId{ get; set; }


        //no estoy seguro de que este vaya aqui
        [Required]
        public int DivisionId { get; set; }


        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }


    }
}
