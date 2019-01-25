using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Catalogue")]
    public class Catalogue
    {
        [Key]
        [Column("id_catalogue"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        [Column("value"), StringLength(30), Required]
        public string Value { get; set; }

        public int GroupCatalogueId { get; set; }
        public GroupCatalogue GroupCatalogue { get; set; }
    }
}
