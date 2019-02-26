using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TaxUsers/")]
    [Produces("application/json")]
    [Authorize]
    public class TaxUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaxUsersController(ApplicationDbContext context)
        {
            _context = context;

        }


        // GET: api/TaxUser
        [HttpGet]
        public IEnumerable<TaxUser> GetTaxUsers()
        {
            return _context.TaxUsers;
        }

        // GET: api/TaxUsers/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxuser = await _context.TaxUsers.FindAsync(id);

            if (taxuser == null)
            {
                return NotFound();
            }

            return Ok(taxuser);
        }


        // POST: api/TaxUsers
        [HttpPost]
        public async Task<IActionResult> PostTaxUsers(int TaxUsersId, [FromBody] TaxUser taxUsers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.TaxUsers.Add(taxUsers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaxUsers", new { id = taxUsers.Id }, taxUsers);
        }

        // PUT: api/TaxUsers/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarranty([FromRoute] int id, [FromBody] TaxUser tax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tax.Id)
            {
                return BadRequest();
            }

            _context.Entry(tax).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxUserExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tax);
        }

        // DELETE: api/TaxUsers/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxUser = await _context.TaxUsers.FindAsync(id);
            if (taxUser == null)
            {
                return NotFound();
            }

            _context.TaxUsers.Remove(taxUser);
            await _context.SaveChangesAsync();

            return Ok(taxUser);
        }


        private bool TaxUserExist(int id)
        {
            return _context.TaxUsers.Any(e => e.Id == id);
        }
    }
}
