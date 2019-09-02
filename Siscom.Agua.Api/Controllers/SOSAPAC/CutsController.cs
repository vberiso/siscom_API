using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Siscom.Agua.Api.Model.SOSAPAC;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

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
                                            .Where(x => x.TypeStateServiceId == 1 && x.Addresses.Any(z => z.TypeAddress == "DIR01"))
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
                        Route = x.Route
                    });
                });
            });


            //List<Suburb> 
            return Ok(suburbs.GroupBy(x => x.Name)
                             .Select(g => g.First())
                             .OrderBy(x => x.Name)
                             .ToList());

        }

        // GET: api/Suburbs/2/5000/
        [HttpGet("{NumOfPeriods}/{Debit}/{Suburbs}")]
        public async Task<IActionResult> GetSuburbs([FromRoute] int NumOfPeriods, [FromRoute] decimal Debit, [FromRoute] string Suburbs)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementBySuburbs]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Suburbs",
                        DbType = DbType.String,
                        Value = Suburbs
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@NumPeriods",
                        DbType = DbType.Int32,
                        Value = NumOfPeriods
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Amount",
                        DbType = DbType.Decimal,
                        Value = Debit
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(result);
                    }
                }
                return Ok(dataTable);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            
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