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
        public TypeRegime()
        {
            Agreements = new HashSet<Agreement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_regime")]
        public int Id { get; set; }
        [Required, StringLength(20), Column("name")]
        public string Name { get; set; }
        [Column("intake_acronym"), StringLength(2), Required]
        public string IntakeAcronym { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
