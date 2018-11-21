using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("System_Log")]
    public class SystemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_system_log")]
        public int Id { get; set; }
        [Column("date_log"), Required]
        public DateTime DateLog { get; set; }
        [Column("controller"), Required]
        public string Controller { get; set; }
        [Column("description"), Required]
        public string Description { get; set; }
        [Column("parameter"), Required]
        public string Parameter { get; set; }
        [Column("action"), Required]
        public string Action { get; set; }
    }
}
