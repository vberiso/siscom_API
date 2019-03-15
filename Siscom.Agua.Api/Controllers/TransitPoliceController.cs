using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitPoliceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransitPoliceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TransitPolice
        [HttpGet]
        public IEnumerable<TransitPolice> GetTransitPolices()
        {
            return _context.TransitPolices;
        }

        // GET: api/TransitPolice/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransitPolice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transitPolice = await _context.TransitPolices.FindAsync(id);

            if (transitPolice == null)
            {
                return NotFound();
            }

            return Ok(transitPolice);
        }

        // PUT: api/TransitPolice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransitPolice([FromRoute] int id, [FromBody] TransitPolice transitPolice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transitPolice.Id)
            {
                return BadRequest();
            }

            _context.Entry(transitPolice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransitPoliceExists(id))
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

        // POST: api/TransitPolice
        [HttpPost]
        public async Task<IActionResult> PostTransitPolice([FromBody] TransitPolice transitPolice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TransitPolices.Add(transitPolice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransitPolice", new { id = transitPolice.Id }, transitPolice);
        }

        // DELETE: api/TransitPolice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransitPolice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transitPolice = await _context.TransitPolices.FindAsync(id);
            if (transitPolice == null)
            {
                return NotFound();
            }

            _context.TransitPolices.Remove(transitPolice);
            await _context.SaveChangesAsync();

            return Ok(transitPolice);
        }

        private bool TransitPoliceExists(int id)
        {
            return _context.TransitPolices.Any(e => e.Id == id);
        }
    }
}