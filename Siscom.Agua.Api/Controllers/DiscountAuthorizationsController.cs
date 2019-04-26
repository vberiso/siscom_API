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
    public class DiscountAuthorizationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscountAuthorizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DiscountAuthorizations
        [HttpGet]
        public IEnumerable<DiscountAuthorization> GetDiscountAuthorizations()
        {
            return _context.DiscountAuthorizations;
        }

        // GET: api/DiscountAuthorizations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discountAuthorization = await _context.DiscountAuthorizations.FindAsync(id);

            if (discountAuthorization == null)
            {
                return NotFound();
            }

            return Ok(discountAuthorization);
        }

        // PUT: api/DiscountAuthorizations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscountAuthorization([FromRoute] int id, [FromBody] DiscountAuthorization discountAuthorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != discountAuthorization.Id)
            {
                return BadRequest();
            }

            _context.Entry(discountAuthorization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountAuthorizationExists(id))
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

        // POST: api/DiscountAuthorizations
        [HttpPost]
        public async Task<IActionResult> PostDiscountAuthorization([FromBody] DiscountAuthorization discountAuthorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DiscountAuthorizations.Add(discountAuthorization);
            await _context.SaveChangesAsync();

            return Ok(discountAuthorization.Id);
        }

        // DELETE: api/DiscountAuthorizations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discountAuthorization = await _context.DiscountAuthorizations.FindAsync(id);
            if (discountAuthorization == null)
            {
                return NotFound();
            }

            _context.DiscountAuthorizations.Remove(discountAuthorization);
            await _context.SaveChangesAsync();

            return Ok(discountAuthorization);
        }

        private bool DiscountAuthorizationExists(int id)
        {
            return _context.DiscountAuthorizations.Any(e => e.Id == id);
        }
    }
}