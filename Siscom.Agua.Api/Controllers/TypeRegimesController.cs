using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TypeRegimesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeRegimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeRegimes
        [HttpGet]
        public IEnumerable<TypeRegime> GetTypeRegimes()
        {
            return _context.TypeRegimes;
        }

        // GET: api/TypeRegimes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeRegime([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeRegime = await _context.TypeRegimes.FindAsync(id);

            if (typeRegime == null)
            {
                return NotFound();
            }

            return Ok(typeRegime);
        }

        // PUT: api/TypeRegimes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeRegime([FromRoute] int id, [FromBody] TypeRegime typeRegime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeRegime.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeRegime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeRegimeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TypeRegimes
        [HttpPost]
        public async Task<IActionResult> PostTypeRegime([FromBody] TypeRegime typeRegime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeRegimes.Add(typeRegime);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeRegime", new { id = typeRegime.Id }, typeRegime);
        }

        // DELETE: api/TypeRegimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeRegime([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeRegime = await _context.TypeRegimes.FindAsync(id);
            if (typeRegime == null)
            {
                return NotFound();
            }

            _context.TypeRegimes.Remove(typeRegime);
            await _context.SaveChangesAsync();

            return Ok(typeRegime);
        }

        private bool TypeRegimeExists(int id)
        {
            return _context.TypeRegimes.Any(e => e.Id == id);
        }
    }
}