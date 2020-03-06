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
    [ApiController]
    [Authorize]
    public class PhonesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Phones
        [HttpGet]
        public IEnumerable<Phones> GetPhones()
        {
            return _context.Phones;
        }

        // GET: api/Phones/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhones([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var phones = await _context.Phones.FindAsync(id);

            if (phones == null)
            {
                return NotFound();
            }

            return Ok(phones);
        }

        // PUT: api/Phones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhones([FromRoute] int id, [FromBody] Phones phones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != phones.Id)
            {
                return BadRequest();
            }

            _context.Entry(phones).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhonesExists(id))
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

        // POST: api/Phones
        [HttpPost]
        public async Task<IActionResult> PostPhones([FromBody] Phones phones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Phones.Add(phones);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhones", new { id = phones.Id }, phones);
        }

        // DELETE: api/Phones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhones([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var phones = await _context.Phones.FindAsync(id);
            if (phones == null)
            {
                return NotFound();
            }

            _context.Phones.Remove(phones);
            await _context.SaveChangesAsync();

            return Ok(phones);
        }

        private bool PhonesExists(int id)
        {
            return _context.Phones.Any(e => e.Id == id);
        }
    }
}