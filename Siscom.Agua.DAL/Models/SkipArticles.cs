using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Skip_Articles")]
    public class SkipArticles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_skip_articles")]
        public int Id { get; set; }

        [Required, StringLength(500), Column("article")]
        public string Article { get; set; }

        [Required, Column("is_active")]
        public bool IsActive { get; set; }

    }
}
