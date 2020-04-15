using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class OrderWorkStatusVM
    {
        public string IdStatus { get; set; }
        public string DateOrderWorkStatus { get; set; }
        public string User { get; set; }
        public int OrderWorkId { get; set; }
    }
}
