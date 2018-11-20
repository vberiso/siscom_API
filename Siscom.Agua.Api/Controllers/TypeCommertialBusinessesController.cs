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
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TypeCommertialBusinessesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeCommertialBusinessesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeCommertialBusinesses
        [HttpGet]
        public IEnumerable<TypeCommertialBusiness> GetTypeCommertialBusinesses()
        {
            return _context.TypeCommertialBusinesses;
        }

        // GET: api/TypeCommertialBusinesses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeCommertialBusiness([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeCommertialBusiness = await _context.TypeCommertialBusinesses.FindAsync(id);

            if (typeCommertialBusiness == null)
            {
                return NotFound();
            }

            return Ok(typeCommertialBusiness);
        }

        // PUT: api/TypeCommertialBusinesses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeCommertialBusiness([FromRoute] int id, [FromBody] TypeCommertialBusiness typeCommertialBusiness)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeCommertialBusiness.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeCommertialBusiness).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeCommertialBusinessExists(id))
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

        // POST: api/TypeCommertialBusinesses
        [HttpPost]
        public async Task<IActionResult> PostTypeCommertialBusiness([FromBody] TypeCommertialBusiness typeCommertialBusiness)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeCommertialBusinesses.Add(typeCommertialBusiness);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeCommertialBusiness", new { id = typeCommertialBusiness.Id }, typeCommertialBusiness);
        }

        // DELETE: api/TypeCommertialBusinesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeCommertialBusiness([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeCommertialBusiness = await _context.TypeCommertialBusinesses.FindAsync(id);
            if (typeCommertialBusiness == null)
            {
                return NotFound();
            }

            _context.TypeCommertialBusinesses.Remove(typeCommertialBusiness);
            await _context.SaveChangesAsync();

            return Ok(typeCommertialBusiness);
        }

        private bool TypeCommertialBusinessExists(int id)
        {
            return _context.TypeCommertialBusinesses.Any(e => e.Id == id);
        }
    }
}