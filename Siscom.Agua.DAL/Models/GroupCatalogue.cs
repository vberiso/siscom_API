using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Group_Catalogue")]
    public class GroupCatalogue
    {
        public GroupCatalogue()
        {
            Catalogues = new HashSet<Catalogue>();
        }

        [Column("id_group_catalogue"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }

        public ICollection<Catalogue> Catalogues { get; set; }
    }
}
