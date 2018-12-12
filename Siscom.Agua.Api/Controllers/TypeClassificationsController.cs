using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TypeClassificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeClassificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeClassifications
        [HttpGet]
        public IEnumerable<TypeClassification> GetTypeClassifications()
        {
            return _context.TypeClassifications;
        }

        // GET: api/TypeClassifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeClassification([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeClassification = await _context.TypeClassifications.FindAsync(id);

            if (typeClassification == null)
            {
                return NotFound();
            }

            return Ok(typeClassification);
        }

        // PUT: api/TypeClassifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeClassification([FromRoute] int id, [FromBody] TypeClassificationsVM typeClassificationsVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeClassificationsVM.Id)
            {
                return BadRequest();
            }

            var typeClassification = await _context.TypeClassifications.FindAsync(typeClassificationsVM.Id);

            typeClassification.Name = typeClassificationsVM.Name;
            typeClassification.IntakeAcronym = typeClassificationsVM.IntakeAcronym;

            _context.Entry(typeClassification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeClassificationExists(id))
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

        // POST: api/TypeClassifications
        [HttpPost]
        public async Task<IActionResult> PostTypeClassification([FromBody] TypeClassificationsVM typeClassificationsVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var typeClassification = new TypeClassification
            {
                Name = typeClassificationsVM.Name,
                IntakeAcronym = typeClassificationsVM.IntakeAcronym
            };

            _context.TypeClassifications.Add(typeClassification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeClassification", new { id = typeClassification.Id }, typeClassification);
        }

        // DELETE: api/TypeClassifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeClassification([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeClassification = await _context.TypeClassifications.FindAsync(id);
            if (typeClassification == null)
            {
                return NotFound();
            }

            _context.TypeClassifications.Remove(typeClassification);
            await _context.SaveChangesAsync();

            return Ok(typeClassification);
        }

        private bool TypeClassificationExists(int id)
        {
            return _context.TypeClassifications.Any(e => e.Id == id);
        }
    }
}