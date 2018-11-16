using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AppSettings
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string IssuerSigningKey { get; set; }
        public string Environment { get; set; }
    }
}
