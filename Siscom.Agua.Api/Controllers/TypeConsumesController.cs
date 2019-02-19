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
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class TypeConsumesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeConsumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeConsumes
        [HttpGet]
        public IEnumerable<TypeConsume> GetTypeConsumes()
        {
            return _context.TypeConsumes;
        }

        // GET: api/TypeConsumes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeConsume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeConsume = await _context.TypeConsumes.FindAsync(id);

            if (typeConsume == null)
            {
                return NotFound();
            }

            return Ok(typeConsume);
        }

        // PUT: api/TypeConsumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeConsume([FromRoute] int id, [FromBody] TypeConsume typeConsume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeConsume.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeConsume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeConsumeExists(id))
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

        // POST: api/TypeConsumes
        [HttpPost]
        public async Task<IActionResult> PostTypeConsume([FromBody] TypeConsume typeConsume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeConsumes.Add(typeConsume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeConsume", new { id = typeConsume.Id }, typeConsume);
        }

        // DELETE: api/TypeConsumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeConsume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeConsume = await _context.TypeConsumes.FindAsync(id);
            if (typeConsume == null)
            {
                return NotFound();
            }

            _context.TypeConsumes.Remove(typeConsume);
            await _context.SaveChangesAsync();

            return Ok(typeConsume);
        }

        private bool TypeConsumeExists(int id)
        {
            return _context.TypeConsumes.Any(e => e.Id == id);
        }
    }
}