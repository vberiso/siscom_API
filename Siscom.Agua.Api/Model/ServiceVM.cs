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
    }
}
