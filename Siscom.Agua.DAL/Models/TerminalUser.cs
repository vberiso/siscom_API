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
        [Column("open_date"), Required]
        public DateTime OpenDate { get; set; }
        [Column("in_operation"), Required]
        public bool InOperation { get; set; }

        [Column("id_terminal")]
        public int TermianlId { get; set; }
        public Terminal Terminal { get; set; }

        [Column("id_user")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
