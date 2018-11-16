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
    [Route("api/States/{StatesId}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TownsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TownsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Towns
        [HttpGet]
        public IEnumerable<Town> GetTowns(int StatesId)
        {
            return _context.Towns.Where(s => s.States.Id == StatesId);
        }

        // GET: api/Towns/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTown([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var town = await _context.Towns.FindAsync(id);

            if (town == null)
            {
                return NotFound();
            }

            return Ok(town);
        }

        // PUT: api/Towns/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTown([FromRoute] int id, [FromBody] Town town)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != town.Id)
            {
                return BadRequest();
            }

            _context.Entry(town).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TownExists(id))
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

        // POST: api/Towns
        [HttpPost]
        public async Task<IActionResult> PostTown([FromBody] Town town)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Towns.Add(town);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTown", new { id = town.Id }, town);
        }

        // DELETE: api/Towns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTown([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var town = await _context.Towns.FindAsync(id);
            if (town == null)
            {
                return NotFound();
            }

            _context.Towns.Remove(town);
            await _context.SaveChangesAsync();

            return Ok(town);
        }

        private bool TownExists(int id)
        {
            return _context.Towns.Any(e => e.Id == id);
        }
    }
}