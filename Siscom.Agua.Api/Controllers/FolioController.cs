using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    // <summary>
    /// End Points Folio
    /// </summary>
    [Route("api/Folio")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class FolioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FolioController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Folio
        /// </summary>
        /// <returns></returns>
        // GET: api/Folio
        [HttpGet]
        public IEnumerable<Folio> GetFolio()
        {
            return _context.Folios;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Folio which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>Folio Model</returns>
        // GET: api/Folio/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Folio = await _context.Folios.FindAsync(id);

            if (Folio == null)
            {
                return NotFound();
            }

            return Ok(Folio);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="folio">Model Folio</param>
        /// <returns></returns>
        // PUT: api/Folio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolio([FromRoute] int id, [FromBody] Folio folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != folio.Id)
            {
                return BadRequest();
            }

            if (!Validate(folio))
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            _context.Entry(folio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)TypeError.Code.Ok, new { Error = string.Format("Modificación realizada con éxito") });
        }

        /// <summary>
        /// This will provide capability add new Folio 
        /// </summary>
        /// <param name="folio">Model Folio</param>
        /// <returns>New Folio added</returns>
        // POST: api/Folio
        [HttpPost]
        public async Task<IActionResult> PostFolio([FromBody] Folio folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Folios.Add(folio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolio", new { id = folio.Id }, folio);
        }

        /// <summary>
        /// This will provide delete for especific ID, of Folio whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/Folio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var folio = await _context.Folios.FindAsync(id);
            if (folio == null)
            {
                return NotFound();
            }

            _context.Folios.Remove(folio);
            await _context.SaveChangesAsync();

            return Ok(folio);
        }

        private bool FolioExists(int id)
        {
            return _context.Folios.Any(e => e.Id == id);
        }

        private bool Validate(Folio folio)
        {
            if (folio.Initial == 0)
                return false;
            if (folio.BranchOffice == null || folio.BranchOffice.Id==0)
                return false;
            return true;
        }
    }
}
