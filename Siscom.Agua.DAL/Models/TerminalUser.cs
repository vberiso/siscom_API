using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Terminal_User")]
    public class TerminalUser
    {
        [Key]
        [Column("id_terminal_user"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        [Column("open_date"), Required]
        public DateTime OpenDate { get; set; }
        [Column("in_operation"), Required]
        public bool InOperation { get; set; }
        
        public Terminal Terminal { get; set; }
        public ApplicationUser User { get; set; }
    }
}
