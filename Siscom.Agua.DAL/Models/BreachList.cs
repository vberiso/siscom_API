using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_List")]
    public class BreachList
    {
        public BreachList()
        {
            BreachDetails = new HashSet<BreachDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_list")]
        public int Id { get; set; }
        [Column("fraction"), Required, StringLength(40)]
        public string Fraction {get;set;}
        [Column("description"), Required, StringLength(100)]
        public string Description {get;set;}
        [Column("min"), Required]
        public int Min {get;set;}
        [Column("min"), Required]
        public int Max {get;set;}
        [Column("have_bonification"), Required]
        public bool HaveBonification {get;set;}
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public int BreachArticleId { get; set; }
        public BreachArticle  BreachArticle { get; set; }

        public ICollection<BreachDetail> BreachDetails { get; set; }
    }
}
