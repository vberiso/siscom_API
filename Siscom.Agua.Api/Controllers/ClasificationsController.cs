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
    /// <summary>
    /// End Points Clasification
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class ClasificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClasificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Clasifications
        /// </summary>
        /// <returns></returns>
        // GET: api/Clasifications
        [HttpGet]
        public IEnumerable<Clasification> GetClasifications()
        {
            return _context.Clasifications;
        }

        /// <summary>
        /// This will provide details for the specific ID, of region which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>Clasification Model</returns>
        // GET: api/Clasifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClasification([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clasification = await _context.Clasifications.FindAsync(id);

            if (clasification == null)
            {
                return NotFound();
            }

            return Ok(clasification);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="clasification">Model Clasification</param>
        /// <returns></returns>
        // PUT: api/Clasifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClasification([FromRoute] int id, [FromBody] Clasification clasification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clasification.Id)
            {
                return BadRequest();
            }

            _context.Entry(clasification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClasificationExists(id))
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

        /// <summary>
        /// This will provide capability add new clasification 
        /// </summary>
        /// <param name="clasification">Model Clasification</param>
        /// <returns>New clasification added</returns>
        // POST: api/Clasifications
        [HttpPost]
        public async Task<IActionResult> PostClasification([FromBody] Clasification clasification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Clasifications.Add(clasification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClasification", new { id = clasification.Id }, clasification);
        }

        /// <summary>
        /// This will provide delete for especific ID, of clasification whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/Clasifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClasification([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clasification = await _context.Clasifications.FindAsync(id);
            if (clasification == null)
            {
                return NotFound();
            }

            _context.Clasifications.Remove(clasification);
            await _context.SaveChangesAsync();

            return Ok(clasification);
        }

        private bool ClasificationExists(int id)
        {
            return _context.Clasifications.Any(e => e.Id == id);
        }
    }
}