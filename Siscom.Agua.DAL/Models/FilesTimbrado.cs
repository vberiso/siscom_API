using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Files_Timbrado")]
    public class FilesTimbrado
    {
        [Key]
        [Column("id_Files_Timbrado"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("path_file"), StringLength(50), Required]
        public string PathFile { get; set; }
        [Column("name_file"), StringLength(50), Required]
        public string NameFile { get; set; }        
        [Column("is_active"), Required]
        public bool IsActive { get; set; }        
        [Column("start_date")]
        public DateTime? StartDate { get; set; }
        [Column("end_date")]
        public DateTime? EndDate { get; set; }
        [Column("pass_key"), StringLength(50)]
        public string PassKey { get; set; }
        [Column("certificate_number"), StringLength(200)]
        public string CertificateNumber { get; set; }
    }
}
