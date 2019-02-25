using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TaxUser/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class TaxUser : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public TaxUser(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Warranty
        [HttpGet]
        public IEnumerable<TaxUser> GetTaxUsers()
        {
            return (System.Collections.Generic.IEnumerable<Siscom.Agua.Api.Controllers.TaxUser>)_context.TaxUsers;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxuser = await _context.TaxUsers.FindAsync(id);

            if (taxuser == null)
            {
                return NotFound();
            }

            return Ok(taxuser);
        }

       

    }
}
