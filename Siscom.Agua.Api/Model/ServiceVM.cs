using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ServiceVM
    {
        public int Id { get; set; }
        [Required, StringLength(25)]
        public string Name { get; set; }
        public Int16 Order { get; set; }
        [Required]
        public bool IsService { get; set; }
        [Required]
        public bool HaveTax { get; set; }
        [Required]
        public bool InAgreement { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
