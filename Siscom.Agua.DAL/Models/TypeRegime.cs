using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Regime")]
    public class TypeRegime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_regime")]
        public int Id { get; set; }
        [Required, StringLength(20), Column("name")]
        public string Name { get; set; }

    }
}
