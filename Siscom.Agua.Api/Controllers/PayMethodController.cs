using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points PayMethod
    /// </summary>
    [Route("api/PayMethod")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class PayMethodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PayMethodController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Pay Method
        /// </summary>
        /// <returns></returns>
        // GET: api/PayMethod
        [HttpGet]
        public IEnumerable<PayMethod> GetPayMethod()
        {
            return _context.PayMethods;
        }

        /// <summary>
        /// This will provide details for the specific ID, of PayMethod which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>PayMethod Model</returns>
        // GET: api/PayMethod/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payMethod = await _context.PayMethods.FindAsync(id);

            if (payMethod == null)
            {
                return NotFound();
            }

            return Ok(payMethod);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="payMethod">Model PayMethod</param>
        /// <returns></returns>
        // PUT: api/PayMethod/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranchOffice([FromRoute] int id, [FromBody] PayMethod payMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(payMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayMethodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)TypeError.Code.Ok, new { Error = string.Format("Modificación realizada con éxito") });
        }

        /// <summary>
        /// This will provide capability add new PayMethod
        /// </summary>
        /// <param name="payMethod">Model PayMethod</param>
        /// <returns>New PayMethod added</returns>
        // POST: api/PayMethod
        [HttpPost]
        public async Task<IActionResult> PostPayMethod([FromBody] PayMethod payMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PayMethods.Add(payMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayMethod", new { id = payMethod.Id }, payMethod);
        }

        /// <summary>
        /// This will provide delete for especific ID, of PayMethod whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/PayMethod/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payMethod = await _context.PayMethods.FindAsync(id);
            if (payMethod == null)
            {
                return NotFound();
            }

            _context.PayMethods.Remove(payMethod);
            await _context.SaveChangesAsync();

            return Ok(payMethod);
        }

        private bool PayMethodExists(int id)
        {
            return _context.BranchOffices.Any(e => e.Id == id);
        }
    }
}
