using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        [Column("id_status",Order =0)]
        public string CodeName { get; set; }
        [Key]
        [Column(Order = 1)]
        public int GroupStatusId { get; set; }

        [Column("description"), StringLength(30), Required]
        public string Description { get; set; }

        public GroupStatus GroupStatus { get; set; }
    }
}
