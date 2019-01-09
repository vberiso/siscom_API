using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Terminal")]
    public class Terminal
    {
        public Terminal()
        {
            TerminalUsers = new HashSet<TerminalUser>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_terminal")]
        public int Id { get; set; }
        [Required, StringLength(20), Column("mac_adress")]
        public string MacAdress { get; set; }
        [Column("cash_box")]
        public decimal CashBox { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        [Column("serial_number"), StringLength(20)]
        public string SerialNumber { get; set; }

        //[ForeignKey("BranchOffice")]
        public int BranchOfficeId { get; set; }
        public BranchOffice BranchOffice { get; set; }

        public ICollection<TerminalUser> TerminalUsers { get; set; }
    }
}
