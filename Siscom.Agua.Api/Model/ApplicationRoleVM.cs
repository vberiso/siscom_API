using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ApplicationRoleVM
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
