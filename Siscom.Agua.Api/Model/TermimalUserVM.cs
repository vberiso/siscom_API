using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TermimalUserVM
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; set; }       
        public bool InOperation { get; set; }
        [Required(ErrorMessage = "Terminal requerida")]
        public int TermianlId { get; set; }
        [Required(ErrorMessage = "Usuario requerido")]
        public string UserId { get; set; }
    }
}
