using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PhotoSyncMobile
    {
        public byte[] Photo { get; set; }
        public string Type { get; set; }
        public DateTime DateTake { get; set; }
    }
}
