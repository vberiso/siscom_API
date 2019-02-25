using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Towns/{TownsId}/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class SuburbsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuburbsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Suburbs
        [HttpGet]
        public IEnumerable<SuburbVM> GetSuburbs(int TownsId)
        {
            if ( _context.Towns.Where(x => x.Id == TownsId).SingleOrDefault() != null)
            {
                var a = _context.Suburbs.Include(r => r.Regions)
                                        .Include(c => c.Clasifications)
                                        .Where(t => t.Towns.Id == TownsId)
                                        .Select(x => new SuburbVM{
                                            Id = x.Id,
                                            Name = x.Name,
                                            ClasificationId = x.Clasifications.Id,
                                            RegionId = x.Regions.Id,
                                            SuburbId = x.Towns.StateId
                                        }).ToList().OrderByDescending(x => x.Name);
                return a;
            }
            else
            {
                return Enumerable.Empty<SuburbVM>();
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

            var suburb = await _context.Suburbs
                                        .Include(r => r.Regions)
                                        .Include(c => c.Clasifications)
                                        .Include(t => t.Towns)
                                        .SingleOrDefaultAsync(i => i.Id == id);

            if (suburb == null)
            {
                return NotFound();
            }

            return Ok(suburb);
        }

        // PUT: api/Suburbs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuburb(int TownsId, [FromRoute] int id, [FromBody] SuburbVM suburbvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != suburbvm.Id)
            {
                return BadRequest();
            }
            var town = await _context.Towns.FindAsync(TownsId);
            if (town == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el municipio") });
            }
            var suburbcontent = await _context.Suburbs.FindAsync(suburbvm.Id);
            if (suburbcontent == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar la colonia") });
            }
            var clas = await _context.Clasifications.FindAsync(suburbvm.ClasificationId);
            var region = await _context.Regions.FindAsync(suburbvm.RegionId);
           
            suburbcontent.Clasifications = clas;
            suburbcontent.Regions = region;
            suburbcontent.Towns = town;
            suburbcontent.Name = suburbvm.Name;
            _context.Entry(suburbcontent).State = EntityState.Modified;

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
        /// <summary>
        /// Creates a Suburb.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "name": "Item1",
        ///        "RegionId": 2
        ///        "ClasificationId": 2
        ///     }
        ///
        /// </remarks>
        // POST: api/Suburbs
        [HttpPost]
        public async Task<IActionResult> PostSuburb(int TownsId, [FromBody] SuburbVM suburb)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var town = await _context.Towns.FindAsync(TownsId);
            if (town == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el municipio") });
            }
            Suburb NewSuburb = new Suburb()
            {
                Name = suburb.Name,
                Towns = town,
                Clasifications = await _context.Clasifications.FindAsync(suburb.ClasificationId),
                Regions = await _context.Regions.FindAsync(suburb.RegionId)
            };
            _context.Suburbs.Add(NewSuburb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuburb", new { id = NewSuburb.Id }, suburb);
        }

        // DELETE: api/Suburbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuburb(int TownsId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var town = await _context.Towns.FindAsync(TownsId);
            if (town == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el municipio") });
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
    }
}