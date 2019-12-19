using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
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
        [HttpPost("{id}")]
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

            //_context.Entry(agreementRulerCalculation).State = EntityState.Modified;

            try
            {
                AgreementRulerCalculation agreementd = agreementRulerCalculation;
                agreementd.IsActive = agreementRulerCalculation.IsActive;

                    AgreementRulerCalculation service = new AgreementRulerCalculation(); 
                    var consulta = await _context.AgreementRulerCalculations.Where(x => x.AgreementId == agreementd.AgreementId && x.ServiceId == agreementd.ServiceId && x.IsActive == agreementd.IsActive).ToListAsync();

                if(consulta.Count < 1)
                {
                    AgreementRulerCalculation log = new AgreementRulerCalculation()
                    {
                        ServiceId = agreementd.ServiceId,
                        Amount = agreementd.Amount,
                        DateIN = agreementd.DateIN,
                        IsActive = agreementd.IsActive,
                        AgreementId = agreementd.AgreementId
                    };
                    _context.AgreementRulerCalculations.Add(log);
                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                if (AgreementRulerCalculationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/AgreementRulerCalculations
        [HttpPost("ActiveAgreementRulerCalculationState/{agreementId}/{serviceId}/{id}")]
        public async Task<IActionResult> ActiveAgreementRulerCalculationState([FromRoute] int agreementId, [FromRoute] int serviceId, [FromRoute] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (_context)
                {
                    var service = await _context.AgreementRulerCalculations.Where(x => x.AgreementId == agreementId && x.ServiceId == serviceId && x.IsActive == true).ToListAsync();
                    var result = _context.AgreementRulerCalculations.SingleOrDefault(x => x.AgreementId == agreementId && x.ServiceId == serviceId && x.IsActive == false && x.Id == Id);
                    if(service.Count < 1)
                    {
                        if (result.IsActive == false)
                        {
                            result.IsActive = true;
                            _context.SaveChanges();
                            return Ok(result);
                        }
                        else
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "La regla de cálculo ya está activa" });
                        }
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Ya existe una regla de cálculo para este concepto" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost("UnactiveAgreementRulerCalculationState/{agreementId}/{serviceId}")]
        public async Task<IActionResult> UnactiveAgreementRulerCalculationState([FromRoute] int agreementId, [FromRoute] int serviceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (_context)
                {
                    var result = _context.AgreementRulerCalculations.SingleOrDefault(x => x.AgreementId == agreementId && x.ServiceId == serviceId && x.IsActive == true);
                    
                        if (result.IsActive == true)
                        {
                            result.IsActive = false;
                            _context.SaveChanges();
                            return Ok(result);
                        }
                        else
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "La regla de cálculo ya está inactiva" });
                        }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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