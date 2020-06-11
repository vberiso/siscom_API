using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class GroupLists
    {
        public GroupLists()
        {
            Lists = new HashSet<Lists>();
        }

        [Key]
        [Column("id_group_lists")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        public ICollection<Lists> Lists { get; set; }
    }
}
