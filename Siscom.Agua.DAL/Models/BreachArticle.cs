using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Article")]
    public class BreachArticle
    {
        public BreachArticle()
        {
            BreachList = new HashSet<BreachList>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_article")]
        public int Id { get; set; }

        [Column("active"), Required, StringLength(30)]
        public string Active { get; set; }

        [Column("article"), Required, StringLength(30)]
        public string Article { get; set; }

        public ICollection<BreachList> BreachList { get; set; }

    }
}
