using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class RepuestaCancelacion
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Uuid { get; set; }
        public DateTime RequestDate { get; set; }
        public string AcuseXmlBase64 { get; set; }
        public DateTime CancelationDate { get; set; }
    }
}
