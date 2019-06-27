using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class BrandModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_brand_model")]
        public int Id { get; set; }
        [Column("brand"), StringLength(30)]
        public string Brand { get; set; }
        [Column("model"), StringLength(30)]
        public string Model { get; set; }
    }
}
