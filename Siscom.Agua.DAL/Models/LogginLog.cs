using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("LogginLog")]
    public class LogginLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_loggingLog")]
        public int Id { get; set; }
        [Column("Fecha")]
        public DateTime Fecha { get; set; }
        [Column("Terminal"), StringLength(30)]
        public string Terminal { get; set; }
        [Column("User"), StringLength(30)]
        public string User { get; set; }
        [Column("VersionSiscom"), StringLength(30)]
        public string VersionSiscom { get; set; }
        [Column("BranchOffice"), StringLength(50)]
        public string BranchOffice { get; set; }
        [Column("UserName"), StringLength(150)]
        public string UserName { get; set; }
    }
}
