using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Commertial_Business")]
    public class TypeCommercialBusiness
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_commertial_business")]
        public int Id { get; set; }
        [Required, StringLength(100), Column("name")]
        public string Name { get; set; }
        [Required, Column("clasification_group")]
        public int ClasificationGroup { get; set; }
        [Column("intake_acronym"), StringLength(2), Required]
        public string IntakeAcronym { get; set; }
    }
}
