using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.SOSAPAC
{
    public class SendCutsVM
    {
        public int Periods { get; set; }
        public decimal Amount { get; set; }
        public int NumNotification { get; set; }
        public string Routes { get; set; }
    }
}
