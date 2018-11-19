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
    /// End Points TypeTransaction
    /// </summary>
    [Route("api/TypeTransaction")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TypeTransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all TypeTransactions
        /// </summary>
        /// <returns></returns>
        // GET: api/TypeTransaction
        [HttpGet]
        public IEnumerable<TypeTransaction> GetTypeTransaction()
        {
            return _context.TypeTransactions;
        }

        /// <summary>
        /// This will provide details for the specific ID, of TypeTransaction which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>TypeTransaction Model</returns>
        // GET: api/TypeTransaction/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeTransaction = await _context.TypeTransactions.FindAsync(id);

            if (typeTransaction == null)
            {
                return NotFound();
            }

            return Ok(typeTransaction);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="typeTransaction">Model TypeTransaction</param>
        /// <returns></returns>
        // PUT: api/TypeTransaction/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranchOffice([FromRoute] int id, [FromBody] TypeTransaction typeTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != typeTransaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeTransaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeTransactionExists(id))
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
        /// This will provide capability add new TypeTransaction
        /// </summary>
        /// <param name="typeTransaction">Model TypeTransaction</param>
        /// <returns>New TypeTransaction added</returns>
        // POST: api/TypeTransaction
        [HttpPost]
        public async Task<IActionResult> PostTypeTransaction([FromBody] TypeTransaction typeTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TypeTransactions.Add(typeTransaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeTransaction", new { id = typeTransaction.Id }, typeTransaction);
        }

        /// <summary>
        /// This will provide delete for especific ID, of TypeTransaction whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/TypeTransaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeTransaction = await _context.TypeTransactions.FindAsync(id);
            if (typeTransaction == null)
            {
                return NotFound();
            }

            _context.TypeTransactions.Remove(typeTransaction);
            await _context.SaveChangesAsync();

            return Ok(typeTransaction);
        }

        private bool TypeTransactionExists(int id)
        {
            return _context.BranchOffices.Any(e => e.Id == id);
        }
    }

}
