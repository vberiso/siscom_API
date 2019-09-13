using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Technical_Role")]
    public class TechnicalRole
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_technical_role")]
        public int Id { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }


        [Column("is_active"), Required]
        public bool IsActive { get; set; }


       
        public ICollection<TechnicalStaff> TechnicalStaffs { get; set; }
    }
}
