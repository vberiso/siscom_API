using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("View")]
    public class View
    {
        public View()
        {
            Controls = new HashSet<Control>();
            ViewProfiles = new HashSet<ViewProfile>();
        }

        [Column("id_view"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("alias"), StringLength(30), Required]
        public string Alias { get; set; }

        public ICollection<Control> Controls { get; set; }
        public ICollection<ViewProfile> ViewProfiles { get; set; }
    }
}
