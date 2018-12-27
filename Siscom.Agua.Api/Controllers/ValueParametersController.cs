using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ValueParametersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ValueParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(string value)
        {
            var today = DateTime.UtcNow.ToLocalTime().Date;
            var b = value.ToUpper();
            var val = _context.SystemParameters.Where(x => x.Name == b && x.IsActive == true && (x.StartDate <= today && x.EndDate >= today)).FirstOrDefault();
            return Ok(val);
        }
    }
}