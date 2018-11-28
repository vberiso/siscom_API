using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/Agreements/{AgreementsId}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class MetersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MetersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Meters
        [Route("Agreements/{AgreementsId}/[Action]")]
        [HttpGet]
        public IEnumerable<Meter> GetMetersByAgreement()
        {
            return _context.Meters;
        }

        // GET: api/Meters/5
        [Route("Agreements/{AgreementsId}/[Action]/{id}")]
        [HttpGet()]
        public async Task<IActionResult> GetMeter([FromRoute] int AgreementsId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(await _context.Agreements.FindAsync(AgreementsId) == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Se ha enviado mal los datos favor de verificar" });
            }
            var meter = await _context.Meters
                                      .Include(x => x.Consumption)
                                      .Include(a => a.Agreement)
                                      .Where(i => i.Id == id && i.Agreement.Id == AgreementsId)
                                      .FirstOrDefaultAsync();

            if (meter == null)
            {
                return NotFound();
            }

            return Ok(meter);
        }

        [HttpGet("GetAllMeters")]
        public IEnumerable<Meter> GetAllMeters()
        {
            return _context.Meters;
        }


        // PUT: api/Meters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeter([FromRoute] int id, [FromBody] Meter meter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != meter.Id)
            {
                return BadRequest();
            }

            _context.Entry(meter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeterExists(id))
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

        //// POST: api/Meters
        //[HttpPost]
        //public async Task<IActionResult> PostMeter([FromBody] Meter meter)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Meters.Add(meter);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (MeterExists(meter.Id))
        //        {
        //            return new StatusCodeResult(StatusCodes.Status409Conflict);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetMeter", new { id = meter.Id }, meter);
        //}

        // DELETE: api/Meters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meter = await _context.Meters.FindAsync(id);
            if (meter == null)
            {
                return NotFound();
            }

            _context.Meters.Remove(meter);
            await _context.SaveChangesAsync();

            return Ok(meter);
        }

        private bool MeterExists(int id)
        {
            return _context.Meters.Any(e => e.Id == id);
        }
    }
}