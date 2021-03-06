using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Branch_Office")]
    public class BranchOffice
    {
        public BranchOffice()
        {
            Terminals = new HashSet<Terminal>();
            Folios = new HashSet<Folio>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_branch_office")]
        public int Id { get; set; }
        [Required, StringLength(30), Column("name")]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("opening")]
        public DateTime Opening { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("closing")]
        public DateTime Closing { get; set; }
        [Required, Column("dont_close")]
        public bool DontClose { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<Terminal> Terminals { get; set; }
        public ICollection<Folio> Folios { get; set; }
    }
}
