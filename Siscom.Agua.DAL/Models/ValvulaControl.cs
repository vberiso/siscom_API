using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("valvula_control")]
    public class ValvulaControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_valvula_control")]
        public int Id { get; set; }
        [Column("description"), StringLength(150)]
        public string Description  { get; set; }
        [Column("reference"), StringLength(300)]
        public string Reference { get; set; }
        [Column("latitude"), StringLength(20)]
        public string Latitude { get; set; }
        [Column("longitude"), StringLength(20)]
        public string Longitude { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
