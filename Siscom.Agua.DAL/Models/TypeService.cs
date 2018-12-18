using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Service")]
    public class TypeService
    {
        public TypeService()
        {
            Agreements = new HashSet<Agreement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_service")]
        public int Id { get; set; }
        [Required, StringLength(10), Column("name")]
        public string Name { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
