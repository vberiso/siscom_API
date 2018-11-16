using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class ApplicationRol : IdentityRole
    {
        public ApplicationRol() : base() { }
        public ApplicationRol(string name) : base(name) { }
        [Column("is_active")]
        public bool IsActive { get; set; }

        public IList<ViewProfile> ViewProfiles { get; set; }
    }
}
