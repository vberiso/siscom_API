using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Transaction")]
    public class TypeTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_transaction")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
    }
}
