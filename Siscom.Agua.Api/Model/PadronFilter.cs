using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PadronFilter
    {

        public int TypeConsume { get; set; }

        public decimal Amount { get; set; }

        public int TypeService { get; set; }

        public int TypeIntake { get; set; }
        /// <summary>
        /// Example 2019-01-01
        /// </summary>
        public string StratDate { get; set; }
        /// <summary>
        /// Example 2019-01-01
        /// </summary>
        public string EndDate { get; set; }

        public int SuburbsId { get; set; }
    }
}
