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

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_list")]
        public int Id { get; set; }

        [Column("active"), Required, StringLength(30)]
        public string Active {get;set;}

        [Column("fraction"), Required, StringLength(40)]
        public string Fraction {get;set;}

        [Column("description"), Required, StringLength(100)]
        public string Description {get;set;}


        [Column("min"), Required, StringLength(30)]
        public string Min {get;set;}


        [Column("min"), Required, StringLength(30)]
        public string Max {get;set;}

        [Column("bonification"), Required, StringLength(100)]
        public string Bonification {get;set;}


        public string BreachArticleId { get; set; }
        public BreachArticle  BreachArticle { get; set; }

        


        


    }
}
