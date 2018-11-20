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
    public class TypeStateServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeStateServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeStateServices
        [HttpGet]
        public IEnumerable<TypeStateService> GetTypeStateServices()
        {
            return _context.TypeStateServices;
        }

        // GET: api/TypeStateServices/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeStateService([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeStateService = await _context.TypeStateServices.FindAsync(id);

            if (typeStateService == null)
            {
                return NotFound();
            }

            return Ok(typeStateService);
        }

        // PUT: api/TypeStateServices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeStateService([FromRoute] int id, [FromBody] TypeStateService typeStateService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeStateService.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeStateService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeStateServiceExists(id))
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

        // POST: api/TypeStateServices
        [HttpPost]
        public async Task<IActionResult> PostTypeStateService([FromBody] TypeStateService typeStateService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeStateServices.Add(typeStateService);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeStateService", new { id = typeStateService.Id }, typeStateService);
        }

        // DELETE: api/TypeStateServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeStateService([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeStateService = await _context.TypeStateServices.FindAsync(id);
            if (typeStateService == null)
            {
                return NotFound();
            }

            _context.TypeStateServices.Remove(typeStateService);
            await _context.SaveChangesAsync();

            return Ok(typeStateService);
        }

        private bool TypeStateServiceExists(int id)
        {
            return _context.TypeStateServices.Any(e => e.Id == id);
        }
    }
}