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
    /// <summary>
    /// End Points BranchOffice
    /// </summary>
    [Route("api/BranchOffice")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class BranchOfficeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BranchOfficeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Branch Office
        /// </summary>
        /// <returns></returns>
        // GET: api/BranchOffice
        [HttpGet]
        public IEnumerable<BranchOffice> GetBranchOffice()
        {
            return _context.BranchOffices;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Branch Office which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>BranchOffice Model</returns>
        // GET: api/BranchOffice/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchOffice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var branchOffice = await _context.BranchOffices.FindAsync(id);

            if (branchOffice == null)
            {
                return NotFound();
            }

            return Ok(branchOffice);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="branchOffice">Model BranchOffice</param>
        /// <returns></returns>
        // PUT: api/BranchOffice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranchOffice([FromRoute] int id, [FromBody] BranchOffice branchOffice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branchOffice.Id)
            {
                return BadRequest();
            }

            _context.Entry(branchOffice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchOfficeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)TypeError.Code.Ok, new { Message = string.Format("Modificación realizada con éxito") });
        }

        /// <summary>
        /// This will provide capability add new Branch Office 
        /// </summary>
        /// <param name="branchOffice">Model Branch Office</param>
        /// <returns>New branchOffice added</returns>
        // POST: api/BranchOffice
        [HttpPost]
        public async Task<IActionResult> PostBranchOffice([FromBody] BranchOffice branchOffice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BranchOffices.Add(branchOffice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBranchOffice", new { id = branchOffice.Id }, branchOffice);
        }

        /// <summary>
        /// This will provide delete for especific ID, of Branch Office whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/BranchOffice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranchOffice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var branchOffice = await _context.BranchOffices.FindAsync(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            _context.BranchOffices.Remove(branchOffice);
            await _context.SaveChangesAsync();

            return Ok(branchOffice);
        }

        private bool BranchOfficeExists(int id)
        {
            return _context.BranchOffices.Any(e => e.Id == id);
        }
    }
}
