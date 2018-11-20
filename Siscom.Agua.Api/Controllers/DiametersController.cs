using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class DiametersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Diameters
        [HttpGet]
        public IEnumerable<Diameter> GetDiameters()
        {
            return _context.Diameters;
        }

        // GET: api/Diameters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diameter = await _context.Diameters.FindAsync(id);

            if (diameter == null)
            {
                return NotFound();
            }

            return Ok(diameter);
        }

        // PUT: api/Diameters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiameter([FromRoute] int id, [FromBody] Diameter diameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(diameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiameterExists(id))
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

        // POST: api/Diameters
        [HttpPost]
        public async Task<IActionResult> PostDiameter([FromBody] Diameter diameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Diameters.Add(diameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiameter", new { id = diameter.Id }, diameter);
        }

        // DELETE: api/Diameters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diameter = await _context.Diameters.FindAsync(id);
            if (diameter == null)
            {
                return NotFound();
            }

            _context.Diameters.Remove(diameter);
            await _context.SaveChangesAsync();

            return Ok(diameter);
        }

        private bool DiameterExists(int id)
        {
            return _context.Diameters.Any(e => e.Id == id);
        }
    }
}