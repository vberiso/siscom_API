using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Consume")]
    public class TypeConsume
    {
        public TypeConsume()
        {
            Agreements = new HashSet<Agreement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_consume")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
