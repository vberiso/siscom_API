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
    public class TypeServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeServices
        [HttpGet]
        public IEnumerable<TypeService> GetTypeServices()
        {
            return _context.TypeServices;
        }

        // GET: api/TypeServices/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeService([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeService = await _context.TypeServices.FindAsync(id);

            if (typeService == null)
            {
                return NotFound();
            }

            return Ok(typeService);
        }

        // PUT: api/TypeServices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeService([FromRoute] int id, [FromBody] TypeService typeService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeService.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeServiceExists(id))
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

        // POST: api/TypeServices
        [HttpPost]
        public async Task<IActionResult> PostTypeService([FromBody] TypeService typeService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeServices.Add(typeService);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeService", new { id = typeService.Id }, typeService);
        }

        // DELETE: api/TypeServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeService([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeService = await _context.TypeServices.FindAsync(id);
            if (typeService == null)
            {
                return NotFound();
            }

            _context.TypeServices.Remove(typeService);
            await _context.SaveChangesAsync();

            return Ok(typeService);
        }

        private bool TypeServiceExists(int id)
        {
            return _context.TypeServices.Any(e => e.Id == id);
        }
    }
}