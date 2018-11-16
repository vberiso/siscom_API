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
    public class TypeIntakesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeIntakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeIntakes
        [HttpGet]
        public IEnumerable<TypeIntake> GetTypeIntakes()
        {
            return _context.TypeIntakes;
        }

        // GET: api/TypeIntakes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeIntake([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeIntake = await _context.TypeIntakes.FindAsync(id);

            if (typeIntake == null)
            {
                return NotFound();
            }

            return Ok(typeIntake);
        }

        // PUT: api/TypeIntakes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeIntake([FromRoute] int id, [FromBody] TypeIntake typeIntake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeIntake.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeIntake).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeIntakeExists(id))
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

        // POST: api/TypeIntakes
        [HttpPost]
        public async Task<IActionResult> PostTypeIntake([FromBody] TypeIntake typeIntake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeIntakes.Add(typeIntake);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeIntake", new { id = typeIntake.Id }, typeIntake);
        }

        // DELETE: api/TypeIntakes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeIntake([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeIntake = await _context.TypeIntakes.FindAsync(id);
            if (typeIntake == null)
            {
                return NotFound();
            }

            _context.TypeIntakes.Remove(typeIntake);
            await _context.SaveChangesAsync();

            return Ok(typeIntake);
        }

        private bool TypeIntakeExists(int id)
        {
            return _context.TypeIntakes.Any(e => e.Id == id);
        }
    }
}