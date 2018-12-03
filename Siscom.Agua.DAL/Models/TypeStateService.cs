using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_State_Service")]
    public class TypeStateService
    {
        public TypeStateService()
        {
            Agreements = new HashSet<Agreement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_state_service")]
        public int Id { get; set; }
        [Required, StringLength(10), Column("name")]
        public string Name { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
