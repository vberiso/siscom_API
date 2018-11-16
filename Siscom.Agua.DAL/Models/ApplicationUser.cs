using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            TerminalUsers = new HashSet<TerminalUser>();
        }

        public ICollection<TerminalUser> TerminalUsers { get; set; }
    }
}
