using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Nombre de usuario requerido")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Llave es requerida")]
        public string Address { get; set; }
    }
}
