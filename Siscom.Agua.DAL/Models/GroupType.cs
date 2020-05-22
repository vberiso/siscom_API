using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Group_Type")]
    public class GroupType
    {
        public GroupType()
        {
            Types = new HashSet<Type>();
        }

        [Column("id_group_type"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("observations"), StringLength(250)]
        public string Observations { get; set; }

        public ICollection<Type> Types { get; set; }
    }
}
