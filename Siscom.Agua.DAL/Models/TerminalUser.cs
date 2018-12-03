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
        public TerminalUser()
        {
            Transactions = new HashSet<Transaction>();
        }

        [Key]
        [Column("id_terminal_user"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        [Column("open_date"), Required]
        public DateTime OpenDate { get; set; }
        [Column("in_operation"), Required]
        public bool InOperation { get; set; }

        [ForeignKey("Terminal")]
        public int TerminalId { get; set; }
        public Terminal Terminal { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
