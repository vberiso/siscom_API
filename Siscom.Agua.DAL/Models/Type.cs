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
        [Column("id_type"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CodeName { get; set; }
        [Column("description"), StringLength(30), Required]
        public string Description { get; set; }

        //[ForeignKey("GroupType")]
        public int GroupTypeId { get; set; }
        public GroupType GroupType { get; set; }
    }
}
