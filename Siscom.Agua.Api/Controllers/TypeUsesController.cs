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
    public class TypeUsesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeUsesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeUses
        [HttpGet]
        public IEnumerable<TypeUse> GetTypeUses()
        {
            return _context.TypeUses;
        }

        // GET: api/TypeUses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeUse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeUse = await _context.TypeUses.FindAsync(id);

            if (typeUse == null)
            {
                return NotFound();
            }

            return Ok(typeUse);
        }

        // PUT: api/TypeUses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeUse([FromRoute] int id, [FromBody] TypeUse typeUse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeUse.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeUse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeUseExists(id))
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

        // POST: api/TypeUses
        [HttpPost]
        public async Task<IActionResult> PostTypeUse([FromBody] TypeUse typeUse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeUses.Add(typeUse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeUse", new { id = typeUse.Id }, typeUse);
        }

        //// DELETE: api/TypeUses/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTypeUse([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var typeUse = await _context.TypeUses.FindAsync(id);
        //    if (typeUse == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TypeUses.Remove(typeUse);
        //    await _context.SaveChangesAsync();

        //    return Ok(typeUse);
        //}

        private bool TypeUseExists(int id)
        {
            return _context.TypeUses.Any(e => e.Id == id);
        }
    }
}