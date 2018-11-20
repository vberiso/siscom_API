using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class FolioVM
    {
      
        public int Id { get; set; }
        public string Range { get; set; }
        public int Folio { get; set; }
        public int IsActive { get; set; }
        public int BranchOffice { get; set; }
    }
}
