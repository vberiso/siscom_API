using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class FundraisingFilter
    {
        /// <summary>
        /// Example 2019-01-01
        /// </summary>
        public string StratDate { get; set; }
        /// <summary>
        /// Example 2019-01-01
        /// </summary>
        public string EndDate { get; set; }
        public string BranchOffice { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
