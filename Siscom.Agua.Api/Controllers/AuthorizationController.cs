using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorizationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> ValidAuthorization([FromQuery] string address)
        {
            string valid = address.Replace(" ", "").Replace(":", "").Replace("-", "");
            Regex r = new Regex("^(?:[0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}){5}[0-9a-fA-F]{2}$");
            if (r.IsMatch(valid))
            {
                var auth = await _context.Authorizations.Where(x => x.MAC == address).SingleOrDefaultAsync();
                if(auth != null)
                    return Redirect("https://siscomayuntamientoweb.azurewebsites.net/login?Token=" + address);
                else
                    //return StatusCode((int)TypeError.Code.Unauthorized, new { Error = "Sin autorización para ingresar al sistema, favor de verificar " });
                    return Redirect("https://siscomayuntamientoweb.azurewebsites.net/coming-soon");
            }
            else
            {
               
                return StatusCode((int)TypeError.Code.Unauthorized, new { Error = "La dirección URL no es valida, favor de verificar " });
            }
        }
    }
}