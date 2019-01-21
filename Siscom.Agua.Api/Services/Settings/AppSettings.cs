using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Services.Settings
{
    public class AppSettings
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string IssuerSigningKey { get; set; }
        public string IssuerExpedient { get; set; }
        public string IssuerName { get; set; }
        public string Environment { get; set; }
        public string FilePath { get; set; }
        public string AccessKey { get; set; }
        public string AccountName { get; set; }
    }
}
