using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Intake")]
    public class TypeIntake
    {
        public TypeIntake()
        {
            Agreements = new HashSet<Agreement>();
            Tariffs = new HashSet<Tariff>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_intake")]
        public int Id { get; set; }
        [Required, StringLength(20), Column("name")]
        public string Name { get; set; }
        [Column("acronym"), StringLength(2), Required]
        public string Acronym { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public ICollection<Agreement> Agreements { get; set; }
        public ICollection<Tariff> Tariffs { get; set; }
    }
}
