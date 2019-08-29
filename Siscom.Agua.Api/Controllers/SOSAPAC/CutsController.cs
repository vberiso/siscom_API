using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Siscom.Agua.Api.Model.SOSAPAC;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    public class CutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CutsController(ApplicationDbContext context)
        {
            
            _context = context;
        }
        // GET: api/Suburbs
        [HttpGet("Suburbs")]
        public async Task<IActionResult> GetSuburbs()
        {
            //var Agreement = await _context.Agreements
            //                        .Include(s => s.Addresses)
            //                            .ThenInclude(su => su.Suburbs)
            //                        .Include(d => d.Debts)
            //                        .Where(x => x.Debts.Any(gs => _context.Statuses
            //                                                              .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && 
            //                                                              (gs.Amount - gs.OnAccount) > 1000))
            //                        .ToListAsync();
            var Agreement = await _context.Agreements
                                            .Include(x => x.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                                    .ThenInclude(c => c.Clasifications)
                                            .ToListAsync();

            List<CutSuburbVM> suburbs = new List<CutSuburbVM>();
            Agreement.ForEach(x =>
            {
                x.Addresses.ToList().ForEach(z =>
                {
                    suburbs.Add(new CutSuburbVM
                    {
                        Id = z.Suburbs.Id,
                        Name = z.Suburbs.Name,
                        Classification = z.Suburbs.Clasifications.Name,
                        ClassificationId = z.Suburbs.ClasificationsId
                    });
                });
            });

            
            //List<Suburb> 
            return Ok(suburbs.GroupBy(x => x.Name)
                             .Select(g => g.First())
                             .OrderBy(x => x.Name)
                             .ToList());

        }

        // GET: api/Suburbs/2/5000
        [HttpGet]
        public async Task<IActionResult> GetSuburbs([FromRoute] int NumOfPeriods, [FromRoute] decimal Debit)
        {

            var debt = await _context.Debts.Include(dd => dd.DebtDetails)
                        .Where(gs => _context.Statuses
                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status)).OrderBy(x => x.FromDate).ToListAsync();
            debt.ForEach(x => {
                //int monts = 0;
                //GetMonthDifference()
            });

            return Ok();
        }

        // GET: api/Suburbs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSuburb([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suburb = await _context.Suburbs.FindAsync(id);

            if (suburb == null)
            {
                return NotFound();
            }

            return Ok(suburb);
        }

        // PUT: api/Suburbs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuburb([FromRoute] int id, [FromBody] Suburb suburb)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != suburb.Id)
            {
                return BadRequest();
            }

            _context.Entry(suburb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuburbExists(id))
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

        // POST: api/Suburbs
        [HttpPost]
        public async Task<IActionResult> PostSuburb([FromBody] Suburb suburb)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Suburbs.Add(suburb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuburb", new { id = suburb.Id }, suburb);
        }

        // DELETE: api/Suburbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuburb([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suburb = await _context.Suburbs.FindAsync(id);
            if (suburb == null)
            {
                return NotFound();
            }

            _context.Suburbs.Remove(suburb);
            await _context.SaveChangesAsync();

            return Ok(suburb);
        }

        private bool SuburbExists(int id)
        {
            return _context.Suburbs.Any(e => e.Id == id);
        }

        private static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
    }
}