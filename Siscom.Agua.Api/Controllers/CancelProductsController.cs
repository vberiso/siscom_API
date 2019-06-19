using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/CancelProducts/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class CancelProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CancelProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCancels([FromRoute] string pAccount)
        {
            var cancels = await _context.CancelProduct.ToListAsync();
                                    
            if (cancels == null)
            {
                return NotFound();
            }
            return Ok(cancels);
        }

        [HttpGet("account/{pAccount}")]
        public async Task<IActionResult> GetCancelsFromAccount([FromRoute] string pAccount)
        {
            var cancels = await _context.CancelProduct
                                    .Where(c => c.Account == pAccount).ToListAsync();
                                 
            if (cancels == null)
            {
                return NotFound();
            }
            return Ok(cancels);
        }

        [HttpGet("date/{pDate}")]
        public async Task<IActionResult> GetCancelsFromDate([FromRoute] string pDate)
        {
            DateTime fechaIni = new DateTime(int.Parse(pDate.Split('-')[0]), int.Parse(pDate.Split('-')[1]), int.Parse(pDate.Split('-')[2]), 0, 0, 0);
            DateTime fechaFin = new DateTime(int.Parse(pDate.Split('-')[0]), int.Parse(pDate.Split('-')[1]), int.Parse(pDate.Split('-')[2]), 23, 59, 59);

            var cancels = await _context.CancelProduct.Where(c => c.RequestDate > fechaIni && c.RequestDate < fechaFin).ToListAsync();

            if (cancels == null)
            {
                return NotFound();
            }
            return Ok(cancels);
        }

        //[HttpGet("PagosPrevios")]
        //public async Task<IActionResult> GetPagosPreviosFromProduct()
        //{

        //}


        [HttpPut("Cancels/{id}")]
        [Authorize]
        public async Task<IActionResult> PutCancelProduct([FromRoute] int id, [FromBody] CancelProduct cancel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cancel.Id)
            {
                return BadRequest();
            }

            _context.Entry(cancel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                if (cancel.Type == "TCA01") //con cuenta
                {
                    //Se edita el estado en los debt
                    var debt = _context.Debts.Find(cancel.DebtId);
                    debt.Status = "ED006"; //Cancelado
                    _context.Entry(debt).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    //se agrega el registro de esta edicion en debt_status
                    DebtStatus ds = new DebtStatus()
                    {
                        id_status = "ED006",
                        DebtStatusDate = DateTime.Now,
                        User = cancel.SupervisorId,
                        DebtId = cancel.DebtId
                    };
                    _context.DebtStatuses.Add(ds);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    var order = _context.OrderSales.Find(cancel.OrderSaleId);
                    order.Status = "EOS05"; //Cancelado
                    _context.Entry(order).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!cancelProductExists(id))
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


        // POST: api/Products
        [HttpPost("add")]
        public async Task<IActionResult> PostCancelProducts([FromBody] CancelProduct pCP)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                CancelProduct elem = _context.CancelProduct.FirstOrDefault(x => x.Account == pCP.Account && x.DebtId == pCP.DebtId && x.CodeConcept == pCP.CodeConcept);
                if(elem == null)
                {
                    _context.CancelProduct.Add(pCP);
                    await _context.SaveChangesAsync();
                }               
            }
            catch(Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.Message;
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "CancelProductsController";
                systemLog.Action = "PostProduct";
                //systemLog.Parameter = JsonConvert.SerializeObject(pDebt);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para salvar el registro" });
            }

            return Ok(pCP.Id);
        }


        private bool cancelProductExists(int id)
        {
            return _context.CancelProduct.Any(e => e.Id == id);
        }

    }
}