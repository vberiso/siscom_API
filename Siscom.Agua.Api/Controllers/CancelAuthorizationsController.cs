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
    public class CancelAuthorizationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CancelAuthorizationsController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: api/CancelAuthorizations
        [HttpGet]
        public IEnumerable<CancelAuthorization> GetCancelAuthorization()
        {
            return _context.CancelAuthorization;
        }

        // GET: api/CancelAuthorizations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCancelAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cancelAuthorization = await _context.CancelAuthorization.FindAsync(id);

            if (cancelAuthorization == null)
            {
                return NotFound();
            }

            return Ok(cancelAuthorization);
        }

        // PUT: api/CancelAuthorizations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCancelAuthorization([FromRoute] int id, [FromBody] CancelAuthorization cancelAuthorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cancelAuthorization.Id)
            {
                return BadRequest();
            }

            _context.Entry(cancelAuthorization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelAuthorizationExists(id))
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

        // POST: api/CancelAuthorizations
        [HttpPost]
        public async Task<IActionResult> PostCancelAuthorization([FromBody] CancelAuthorization cancelAuthorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CancelAuthorization.Add(cancelAuthorization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCancelAuthorization", new { id = cancelAuthorization.Id }, cancelAuthorization);
        }

        // DELETE: api/CancelAuthorizations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancelAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cancelAuthorization = await _context.CancelAuthorization.FindAsync(id);
            if (cancelAuthorization == null)
            {
                return NotFound();
            }

            _context.CancelAuthorization.Remove(cancelAuthorization);
            await _context.SaveChangesAsync();

            return Ok(cancelAuthorization);
        }

        private bool CancelAuthorizationExists(int id)
        {
            return _context.CancelAuthorization.Any(e => e.Id == id);
        }
    }
}