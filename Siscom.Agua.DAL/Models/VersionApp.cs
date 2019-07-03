using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("version_app")]
    public class VersionApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("version"), StringLength(10)]
        public string Version { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("platform"), StringLength(10)]
        public string Platform { get; set; }
        [Column("publish_date")]
        public DateTime PublishDate { get; set; }
    }
}
