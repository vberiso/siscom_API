using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Warranty/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class WarrantyController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public WarrantyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Warranty
        [HttpGet]
        public IEnumerable<Warranty> GetWarranties()
        {
            return _context.Warranties;
        }

        // GET: api/Warranty/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarranty([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warranty = await _context.Warranties.FindAsync(id);

            if (warranty == null)
            {
                return NotFound();
            }

            return Ok(warranty);
        }


        // POST: api/Warranty
        [HttpPost]
        public async Task<IActionResult> PostWarranty(int WarrantyId, [FromBody] Warranty warranty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarranty", new { id = warranty.Id }, warranty);
        }

        // PUT: api/Warranty/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarranty([FromRoute] int id, [FromBody] Warranty warranty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != warranty.Id)
            {
                return BadRequest();
            }

            _context.Entry(warranty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarrantyExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(warranty);
        }

        // DELETE: api/Warranty/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarrranty([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null)
            {
                return NotFound();
            }

            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync();

            return Ok(warranty);
        }


        private bool WarrantyExist(int id)
        {
            return _context.Warranties.Any(e => e.Id == id);
        }
    }
}
