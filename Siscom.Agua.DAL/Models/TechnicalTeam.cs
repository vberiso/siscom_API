using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Technical_Team")]
   public class TechnicalTeam
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_technical_team")]
        public int Id { get; set; }
        [Column("description"), Required, StringLength(150)]
        public string Description { get; set; }
        [Column("name"), Required, StringLength(60)]
        public string Name { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        public ICollection<TechnicalStaff> TechnicalStaffs { get; set; }

    }
}
