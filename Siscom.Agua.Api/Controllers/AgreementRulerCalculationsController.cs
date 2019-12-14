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
    public class AgreementRulerCalculationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AgreementRulerCalculationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AgreementRulerCalculations
        [HttpGet]
        public IEnumerable<AgreementRulerCalculation> GetAgreementRulerCalculations()
        {
            return _context.AgreementRulerCalculations;
        }

        // GET: api/AgreementRulerCalculations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgreementRulerCalculation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agreementRulerCalculation = _context.AgreementRulerCalculations.Where(x => x.AgreementId == id);

            if (agreementRulerCalculation == null)
            {
                return NotFound();
            }

            return Ok(agreementRulerCalculation);
        }

        // PUT: api/AgreementRulerCalculations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgreementRulerCalculation([FromRoute] int id, [FromBody] AgreementRulerCalculation agreementRulerCalculation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agreementRulerCalculation.AgreementId)
            {
                return BadRequest();
            }

            _context.Entry(agreementRulerCalculation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgreementRulerCalculationExists(id))
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

        // POST: api/AgreementRulerCalculations
        [HttpPost]
        public async Task<IActionResult> PostAgreementRulerCalculation([FromBody] AgreementRulerCalculation agreementRulerCalculation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AgreementRulerCalculations.Add(agreementRulerCalculation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAgreementRulerCalculation", new { id = agreementRulerCalculation.AgreementId }, agreementRulerCalculation);
        }

        // DELETE: api/AgreementRulerCalculations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgreementRulerCalculation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agreementRulerCalculation = await _context.AgreementRulerCalculations.FindAsync(id);
            if (agreementRulerCalculation == null)
            {
                return NotFound();
            }

            _context.AgreementRulerCalculations.Remove(agreementRulerCalculation);
            await _context.SaveChangesAsync();

            return Ok(agreementRulerCalculation);
        }

        private bool AgreementRulerCalculationExists(int id)
        {
            return _context.AgreementRulerCalculations.Any(e => e.AgreementId == id);
        }
    }
}