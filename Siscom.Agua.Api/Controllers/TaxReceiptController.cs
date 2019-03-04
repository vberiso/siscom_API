using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TaxReceipt/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Authorize]
    public class TaxReceiptController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaxReceiptController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: api/TaxReceipts
        [HttpGet]
        public IEnumerable<TaxReceipt> GetTaxReceipts()
        {
            return _context.TaxReceipts;
        }

        // GET: api/TaxReceipts/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxReceipt([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxReceipt = await _context.TaxReceipts.FindAsync(id);

            if (taxReceipt == null)
            {
                return NotFound();
            }

            return Ok(taxReceipt);
        }


        // POST: api/TaxReceipts
        [HttpPost]
        public async Task<IActionResult> PostTaxReceipt(int TaxReceiptId, [FromBody] TaxReceipt taxReceipt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try{
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (string.IsNullOrEmpty(taxReceipt.UserId))
                    {
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Ingresar id de usuario" });

                    }

                    if (string.IsNullOrEmpty(taxReceipt.FielXML))
                    {
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Ingresar archivo xml" });
                    }

                    if (string.IsNullOrEmpty(taxReceipt.XML))
                    {
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Ingresar xml" });
                    }



                    _context.TaxReceipts.Add(taxReceipt);
                    await _context.SaveChangesAsync();


                    scope.Complete();
                
                    }

                }catch (Exception e){
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(taxReceipt);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar la factura" });

            }

            //TaxReceipt NewTaxReceipts = new TaxReceipt();

            //var id = await _context.a.FindAsync(taxReceipt.UserId);


            //NewTaxReceipts.TaxUser = taxu;

           

            return CreatedAtAction("GetTaxReceipts", new { id = taxReceipt.Id }, taxReceipt);
        }

        // PUT: api/TaxReceipts/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxReceipt([FromRoute] int id, [FromBody] TaxReceipt tax)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try{
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)){

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
                        if (!TaxReceiptExist(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    scope.Complete();

                }



            }catch (Exception e){

                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(tax);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar la factura" });
            }


            return Ok(tax);
        }

        // DELETE: api/TaxReceipts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxReceipt([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxReceipt = await _context.TaxReceipts.FindAsync(id);
            if (taxReceipt == null)
            {
                return NotFound();
            }

            _context.TaxReceipts.Remove(taxReceipt);
            await _context.SaveChangesAsync();

            return Ok(taxReceipt);
        }


        private bool TaxReceiptExist(int id)
        {
            return _context.TaxReceipts.Any(e => e.Id == id);
        }
    }

}


