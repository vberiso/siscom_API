using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class AccountsAccumulated
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_account_accumulated")]
        public int Id { get; set; }
        [Column("accumulated"), Required]
        public int Accumulated { get; set; }

        [Column("year"), Required]
        public int Year { get; set; }

        [Column("mes"), Required]
        public int Mes { get; set; }

       
    }
}
