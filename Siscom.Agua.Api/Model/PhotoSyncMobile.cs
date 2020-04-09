using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PhotoSyncMobile
    {
        public string Photo { get; set; }
        [DataType(DataType.Date)]
        public string DateTake { get; set; }
        public string Type { get; set; }
    }
}
