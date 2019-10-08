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
    public class PostalMxesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostalMxesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostalMxes
        [HttpGet]
        public IEnumerable<PostalMx> GetPostalMx()
        {
            return _context.PostalMx;
        }

        // GET: api/PostalMxes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostalMx([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postalMx = await _context.PostalMx.FindAsync(id);

            if (postalMx == null)
            {
                return NotFound();
            }

            return Ok(postalMx);
        }

        // PUT: api/PostalMxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostalMx([FromRoute] int id, [FromBody] PostalMx postalMx)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != postalMx.Id)
            {
                return BadRequest();
            }

            _context.Entry(postalMx).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostalMxExists(id))
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

        // POST: api/PostalMxes
        [HttpPost]
        public async Task<IActionResult> PostPostalMx([FromBody] PostalMx postalMx)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PostalMx.Add(postalMx);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PostalMxExists(postalMx.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPostalMx", new { id = postalMx.Id }, postalMx);
        }

        // DELETE: api/PostalMxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostalMx([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postalMx = await _context.PostalMx.FindAsync(id);
            if (postalMx == null)
            {
                return NotFound();
            }

            _context.PostalMx.Remove(postalMx);
            await _context.SaveChangesAsync();

            return Ok(postalMx);
        }

        private bool PostalMxExists(int id)
        {
            return _context.PostalMx.Any(e => e.Id == id);
        }
    }
}