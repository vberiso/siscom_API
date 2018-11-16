using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Siscom.Agua.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Manager")]
    public class ManagerController : Controller
    {
        [HttpGet("/Error"), HttpPost("/Error")]
        public IActionResult Error(int status = 400)
        {
            if (HttpContext.Request.Headers.ContainsKey("Accept") &&
                HttpContext.Request.Headers["Accept"].Contains("application/json"))
            {
                if (status == 403)
                {
                    // TODO: There is no way to get claim here!
                    return new JsonResult(new { error = "FORBIDDEN" });
                }
                else if (status == 401)
                {
                    return new JsonResult(new { error = "LOGIN_REQUIRED" });
                }
                else
                {
                    return new JsonResult(new { error = "UNDEFINED_ERROR" });
                }
            }
            return BadRequest();
            // HTML Version if you wish ...
        }
    }
}