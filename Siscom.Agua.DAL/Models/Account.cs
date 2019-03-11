using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_account")]
        public int Id { get; set; }
        [Column("prefix")]
        public string Prefix { get; set; }
        [Column("secuential"), Required]
        public int Secuential { get; set; }
        [Column("suffixes")]
        public string Suffixes { get; set; }
        [Column("is_active"), Required]        
        public bool IsActive { get; set; }


    }
}
