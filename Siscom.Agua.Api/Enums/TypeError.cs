using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Enums
{
    public class TypeError
    {
        public enum Code
        {
            PartialContent = 206,
            Conflict = 409,
            NoContent =204,
            Ok = 200
        }
    }
}
