using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TreeListItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string TipeService { get; set; }
        public string AccountNumber { get; set; }
        public string CodeConcept { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
