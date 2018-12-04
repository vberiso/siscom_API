using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdressesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Adresses
        //[HttpGet]
        //public IEnumerable<Adress> GetAdresses()
        //{
        //    return _context.Adresses;
        //}

        // GET: api/Adresses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adress = await _context.Adresses.FindAsync(id);

            if (adress == null)
            {
                return NotFound();
            }

            return Ok(adress);
        }

        // PUT: api/Adresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdress([FromRoute] int id, [FromBody] Address adress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adress.Id)
            {
                return BadRequest();
            }

            _context.Entry(adress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdressExists(id))
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

        // POST: api/Adresses
        [HttpPost]
        public async Task<IActionResult> PostAdress([FromBody] AdressVM adress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Address NewAdress = new Address()
            {
                Street = adress.Street,
                Outdoor = adress.Outdoor,
                Indoor = adress.Indoor,
                Zip = adress.Zip,
                Reference = adress.Reference,
                Lat = adress.Lat,
                Lon = adress.Lon,
                TypeAddress = adress.TypeAddress
            };

            _context.Adresses.Add(NewAdress);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdress", new { id = adress.Id }, adress);
        }

        // DELETE: api/Adresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adress = await _context.Adresses.FindAsync(id);
            if (adress == null)
            {
                return NotFound();
            }

            _context.Adresses.Remove(adress);
            await _context.SaveChangesAsync();

            return Ok(adress);
        }

        private bool AdressExists(int id)
        {
            return _context.Adresses.Any(e => e.Id == id);
        }
    }
}