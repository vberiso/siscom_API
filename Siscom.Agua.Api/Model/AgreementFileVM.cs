using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementFileVM
    {
        public IFormFile File { get; set; }
        public AgrementFileData FileData { get; set; }
    }

    public class AgrementFileData
    {
        public int AgreementId { get; set; }
        public string TypeFile { get; set; }
        [StringLength(250)]
        public string Observation { get; set; }
    }
}
