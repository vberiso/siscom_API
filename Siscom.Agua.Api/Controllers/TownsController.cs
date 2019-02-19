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
    [Route("api/States/{StatesId}/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TownsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TownsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Towns
        [HttpGet]
        public IEnumerable<TownVM> GetTowns(int StatesId)
        {
            return _context.Towns.Where(s => s.States.Id == StatesId).Select(x => 
            new TownVM {Id = x.Id, Name = x.Name }).OrderByDescending(s => s.Name);
        }

        // GET: api/Towns/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTown([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            var town = await _context.Towns.FindAsync(id);

            if (town == null)
            {
                return NotFound();
            }

            return Ok(town);
        }

        // PUT: api/Towns/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTown(int StatesId, [FromRoute] int id, [FromBody] TownVM town)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != town.Id)
            {
                return BadRequest();
            }

            var state = await _context.States.FindAsync(StatesId);
            if (state == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el estado") });
            }

            var towncontent = await _context.Towns.FindAsync(town.Id);
            if (towncontent == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el munucipio") });
            }

            towncontent.Name = town.Name;
            towncontent.States = state;
            _context.Entry(towncontent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TownExists(id))
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

        // POST: api/Towns
        [HttpPost]
        public async Task<IActionResult> PostTown(int StatesId, [FromBody] TownVM town)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var state = await _context.States.FindAsync(StatesId);
            if (state == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el estado") });
            }
            Town NewTown = new Town()
            {
                Name = town.Name,
                States = state
            };
            _context.Towns.Add(NewTown);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTown", new { id = town.Id }, town);
        }

        // DELETE: api/Towns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTown(int StatesId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var state = await _context.States.FindAsync(StatesId);
            if (state == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el estado") });
            }

            var town = await _context.Towns.FindAsync(id);
            if (town == null)
            {
                return NotFound();
            }

            _context.Towns.Remove(town);
            await _context.SaveChangesAsync();

            return Ok(town);
        }

        private bool TownExists(int id)
        {
            return _context.Towns.Any(e => e.Id == id);
        }
    }
}