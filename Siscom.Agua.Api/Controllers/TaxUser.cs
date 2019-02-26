
using System.Collections.Generic;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;

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

        [HttpGet]
        public Microsoft.EntityFrameworkCore.DbSet<DAL.Models.TaxUser> GetTaxUsers()
        {
           return _context.TaxUsers;
        }

        // GET: api/Warranty
        //[HttpGet]
        //public IEnumerable<TaxUser> GetTaxUsers()
        //{
        //    return (System.Collections.Generic.IEnumerable<Siscom.Agua.Api.Controllers.TaxUser>)_context.TaxUsers;
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTaxUser([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var taxuser = await _context.TaxUsers.FindAsync(id);

        //    if (taxuser == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(taxuser);
        //}



    }
}
