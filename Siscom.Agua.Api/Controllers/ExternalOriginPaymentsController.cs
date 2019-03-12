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
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class ExternalOriginPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExternalOriginPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExternalOriginPayments
        [HttpGet]
        public IEnumerable<ExternalOriginPayment> GetExternalOriginPayments()
        {
            return _context.ExternalOriginPayments;
        }

        // GET: api/ExternalOriginPayments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExternalOriginPayment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var externalOriginPayment = await _context.ExternalOriginPayments.FindAsync(id);

            if (externalOriginPayment == null)
            {
                return NotFound();
            }

            return Ok(externalOriginPayment);
        }

        // PUT: api/ExternalOriginPayments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExternalOriginPayment([FromRoute] int id, [FromBody] ExternalOriginPayment externalOriginPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != externalOriginPayment.Id)
            {
                return BadRequest();
            }

            _context.Entry(externalOriginPayment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExternalOriginPaymentExists(id))
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

        // POST: api/ExternalOriginPayments
        [HttpPost]
        public async Task<IActionResult> PostExternalOriginPayment([FromBody] ExternalOriginPayment externalOriginPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ExternalOriginPayments.Add(externalOriginPayment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExternalOriginPayment", new { id = externalOriginPayment.Id }, externalOriginPayment);
        }

        // DELETE: api/ExternalOriginPayments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExternalOriginPayment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var externalOriginPayment = await _context.ExternalOriginPayments.FindAsync(id);
            if (externalOriginPayment == null)
            {
                return NotFound();
            }

            _context.ExternalOriginPayments.Remove(externalOriginPayment);
            await _context.SaveChangesAsync();

            return Ok(externalOriginPayment);
        }

        private bool ExternalOriginPaymentExists(int id)
        {
            return _context.ExternalOriginPayments.Any(e => e.Id == id);
        }
    }
}