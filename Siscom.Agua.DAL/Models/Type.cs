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
        [Key]
        [Column("id_type", Order = 0)]
        public string CodeName { get; set; }
        [Key]
        [Column(Order = 1)]
        public int GroupTypeId { get; set; }

        [Column("description"), StringLength(30), Required]
        public string Description { get; set; }
        public GroupType GroupType { get; set; }
    }
}
