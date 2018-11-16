using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Towns/{TownsId}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class SuburbsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuburbsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Suburbs
        [HttpGet]
        public IEnumerable<Suburb> GetSuburbs(int TownsId)
        {
            if ( _context.Towns.Where(x => x.Id == TownsId).SingleOrDefault() != null)
            {
                var a = _context.Suburbs.Include(r => r.Regions).Include(c => c.Clasifications).Where(t => t.Towns.Id == TownsId).ToList();
                return a;
            }
            else
            {
                return Enumerable.Empty<Suburb>();
            }
            
        }

        // GET: api/Suburbs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSuburb([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suburb = await _context.Suburbs.FindAsync(id);

            if (suburb == null)
            {
                return NotFound();
            }

            return Ok(suburb);
        }

        // PUT: api/Suburbs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuburb([FromRoute] int id, [FromBody] Suburb suburb)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != suburb.Id)
            {
                return BadRequest();
            }

            _context.Entry(suburb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuburbExists(id))
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

        // POST: api/Suburbs
        [HttpPost]
        public async Task<IActionResult> PostSuburb([FromBody] Suburb suburb)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Suburbs.Add(suburb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuburb", new { id = suburb.Id }, suburb);
        }

        // DELETE: api/Suburbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuburb([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suburb = await _context.Suburbs.FindAsync(id);
            if (suburb == null)
            {
                return NotFound();
            }

            _context.Suburbs.Remove(suburb);
            await _context.SaveChangesAsync();

            return Ok(suburb);
        }

        private bool SuburbExists(int id)
        {
            return _context.Suburbs.Any(e => e.Id == id);
        }
    }
}