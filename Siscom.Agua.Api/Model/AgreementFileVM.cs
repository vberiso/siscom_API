using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementFileVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string extension { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
