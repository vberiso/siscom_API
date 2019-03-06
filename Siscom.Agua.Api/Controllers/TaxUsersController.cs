using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TaxUsers/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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

        [HttpGet("Search/{name}")]
        public async Task<IActionResult> GetSearch([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxuser = await _context.TaxUsers.Where(n => n.Name == name).ToListAsync();

            if (taxuser.Count == 0)
            {

                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El usuario no existe" });

            }

            return Ok(taxuser);
        }

        [HttpGet("SearchRFC/{rfc}")]
        public async Task<IActionResult>GetSearchR([FromRoute] string rfc){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var tax = await _context.TaxUsers.Where(r => r.RFC == rfc).ToListAsync();

            if(tax.Count == 0){

                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El usuario no existe" });
            }

            return Ok(tax);

        }

        [HttpGet("SearchCurp/{curp}")]
        public async Task<IActionResult>GetSearchC([FromRoute] string curp){

            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var taxc = await _context.TaxUsers.Where(c => c.CURP == curp).ToListAsync();
            if(taxc.Count == 0 ){
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El usuario no existe" });
            }

            return Ok(taxc);
        }

        [HttpGet("SearchPhone/{phone}")]
        public async Task<IActionResult>GetSearchP([FromRoute] string phone){
            if (!ModelState.IsValid){
                return BadRequest(ModelState);

            }

            var taxp = await _context.TaxUsers.Where(p => p.PhoneNumber == phone).ToListAsync();

            if (taxp.Count == 0){
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El usuario no existe" });
            }

            return Ok(taxp);
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
