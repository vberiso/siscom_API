using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class Lists
    {
        [Key]
        [Column("id_lists")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("descripction")]
        [StringLength(50)]
        public string Description { get; set; }
        [Column("value_number")]
        public decimal ValueNumber { get; set; }
        [Column("value_text")]
        public string ValueText { get; set; }
        [Column("associated_type")]
        [StringLength(5)]
        public string AssosiatedType { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        public int GroupListsId { get; set; }
        public GroupLists GroupLists { get; set; }
    }
}
