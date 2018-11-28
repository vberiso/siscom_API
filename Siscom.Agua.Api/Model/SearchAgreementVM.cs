using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class SearchAgreementVM
    {
        [Required]
        public int Type { get; set; }
        [Required]
        public string StringSearch { get; set; }
    }
}
