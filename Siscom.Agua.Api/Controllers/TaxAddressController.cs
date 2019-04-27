using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using System;
using Siscom.Agua.DAL.Models;
using System.Transactions;
using Siscom.Agua.Api.Services.Extension;
using Newtonsoft.Json;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Enums;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TaxAddress/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class TaxAddressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaxAddressController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public IEnumerable<TaxAddress> GetTaxAddresses()
        {
            return _context.TaxAddresses;
        }

        // GET: api/TaxAddress
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxAddress = await _context.TaxAddresses.FindAsync(id);

            if (taxAddress == null)
            {
                return NotFound();
            }

            return Ok(taxAddress);
        }


        // POST: api/TaxAddress
        [HttpPost]
        public async Task<IActionResult> PostTaxAddress(int TaxAddressId, [FromBody] TaxAddress taxAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.TaxAddresses.Add(taxAddress);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaxAddress", new { id = taxAddress.Id }, taxAddress);
        }

        // PUT: api/TaxAddress/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxAddress([FromRoute] int id, [FromBody] TaxAddress taxAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != taxAddress.Id)
                    {
                        return BadRequest();
                    }

                    _context.Entry(taxAddress).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TaxAddressExist(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    scope.Complete();
                    return Ok(taxAddress);
                }

            }
            catch(Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(taxAddress);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar dirección" });

            }

           
        }

        // DELETE: api/TaxAddress/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxAddress = await _context.TaxAddresses.FindAsync(id);
            if (taxAddress == null)
            {
                return NotFound();
            }

            _context.TaxAddresses.Remove(taxAddress);
            await _context.SaveChangesAsync();

            return Ok(taxAddress);
        }


        private bool TaxAddressExist(int id)
        {
            return _context.TaxAddresses.Any(e => e.Id == id);
        }
    }
}

