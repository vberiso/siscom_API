using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            TerminalUsers = new HashSet<TerminalUser>();
            AgreementLogs = new HashSet<AgreementLog>();
            AgreementFiles = new HashSet<AgreementFile>();
            Breaches = new HashSet<Breach>();
            AssignmentTickets = new HashSet<AssignmentTicket>();
        }

        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("middle_name"), StringLength(50), Required]
        public string LastName { get; set; }
        [Column("last_name"), StringLength(50), Required]
        public string SecondLastName { get; set; }
        [Column("id_divition")]
        public int DivitionId { get; set; }

        public ICollection<TerminalUser> TerminalUsers { get; set; }
        public ICollection<AgreementLog> AgreementLogs { get; set; }
        public ICollection<AgreementFile> AgreementFiles { get; set; }
        public ICollection<Breach> Breaches { get; set; }
        public ICollection<AssignmentTicket>  AssignmentTickets { get; set; }
    }
}
