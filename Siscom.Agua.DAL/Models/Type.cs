using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type")]
    public class Type
    {
        [Column("id_type"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(5), Required]
        public string Name { get; set; }
        [Column("description"), StringLength(30), Required]
        public string Description { get; set; }

        public GroupType GroupType { get; set; }
    }
}
