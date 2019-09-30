using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class ReasonCatalog
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_reason-catalog")]
        public int Id { get; set; }
        [Column("description"), MinLength(5), Required]
        public string Description { get; set; }

        [Column("IsActive"), Required]
        public bool IsActive { get; set; }
        
        [Column("type"), Required]
        public string Type { get; set; }

        public ICollection<OrderWorkReasonCatalog> OrderWorkReasonCatalogs { get; set; }
    }
}
