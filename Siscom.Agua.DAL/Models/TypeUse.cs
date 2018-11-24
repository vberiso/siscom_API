using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Use")]
    public class TypeUse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_use")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Column("intake_acronym"), StringLength(2), Required]
        public string IntakeAcronym { get; set; }
    }
}
