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
    public class TypePeriodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypePeriodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypePeriods
        [HttpGet]
        public IEnumerable<TypePeriod> GetTypePeriods()
        {
            return _context.TypePeriods;
        }

        // GET: api/TypePeriods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypePeriod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typePeriod = await _context.TypePeriods.FindAsync(id);

            if (typePeriod == null)
            {
                return NotFound();
            }

            return Ok(typePeriod);
        }

        // PUT: api/TypePeriods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypePeriod([FromRoute] int id, [FromBody] TypePeriod typePeriod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typePeriod.Id)
            {
                return BadRequest();
            }

            _context.Entry(typePeriod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypePeriodExists(id))
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

        // POST: api/TypePeriods
        [HttpPost]
        public async Task<IActionResult> PostTypePeriod([FromBody] TypePeriod typePeriod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypePeriods.Add(typePeriod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypePeriod", new { id = typePeriod.Id }, typePeriod);
        }

        // DELETE: api/TypePeriods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypePeriod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typePeriod = await _context.TypePeriods.FindAsync(id);
            if (typePeriod == null)
            {
                return NotFound();
            }

            _context.TypePeriods.Remove(typePeriod);
            await _context.SaveChangesAsync();

            return Ok(typePeriod);
        }

        private bool TypePeriodExists(int id)
        {
            return _context.TypePeriods.Any(e => e.Id == id);
        }
    }
}