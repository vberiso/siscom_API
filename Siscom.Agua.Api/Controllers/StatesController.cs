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
    [Route("api/Countries/{CountriesId}/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class StatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/States
        [HttpGet]
        public IEnumerable<StateVM> GetStates(int CountriesId)
        {
            return _context.States.Include(x => x.Countries)
                    .Where(c => c.Countries.Id == CountriesId)
                    .Select(s => new StateVM {
                    Id = s.Id,
                    Name = s.Name,
                    abbreviation = s.Countries.Abbreviation
                }).ToList();
        }

        // GET: api/States/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetState([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var state = await _context.States.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        // PUT: api/States/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutState(int CountriesId, [FromRoute] int id, [FromBody] StateVM state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != state.Id)
            {
                return BadRequest();
            }

            var country = await _context.Countries.FindAsync(CountriesId);
            if (country == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el pais") });
            }
            var statte = await _context.States.FindAsync(state.Id);
            if(statte == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el estado") });
            }
            statte.Name = state.Name;
            statte.Countries = country;
            _context.Entry(statte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
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

        // POST: api/States
        [HttpPost]
        public async Task<IActionResult> PostState(int CountriesId, [FromBody] StateVM state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var country = await _context.Countries.FindAsync(CountriesId);
            if (country == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el pais") });
            }

            State NewState = new State()
            {
                Name = state.Name,
                Countries = country
            };
            _context.States.Add(NewState);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetState", new { id = state.Id }, state);
        }

        // DELETE: api/States/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteState(int CountriesId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var country = await _context.Countries.FindAsync(CountriesId);
            if (country == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el pais") });
            }

            var state = await _context.States.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }

            _context.States.Remove(state);
            await _context.SaveChangesAsync();

            return Ok(state);
        }

        private bool StateExists(int id)
        {
            return _context.States.Any(e => e.Id == id);
        }
    }
}