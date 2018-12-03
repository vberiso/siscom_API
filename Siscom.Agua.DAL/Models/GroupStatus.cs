using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Group_Status")]
    public class GroupStatus
    {
        public GroupStatus()
        {
            Statuses = new HashSet<Status>();
        }

        [Column("id_group_type"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }

        public ICollection<Status> Statuses { get; set; }
    }   
}
