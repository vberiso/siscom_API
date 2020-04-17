using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class LocationOfAttentionOrderWork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_Location")]
        public int Id { get; set; }
        [Column("latitude"), StringLength(20)]
        public string Latitude { get; set; }
        [Column("longitude"), StringLength(20)]
        public string Longitude { get; set; }
        [Column("type"), StringLength(10)]
        public string Type { get; set; }
        public ICollection<LocationOrderWork> LocationOrderWorks { get; set; }
    }
}
