using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class DiscountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Discounts
        [HttpGet]
        public IEnumerable<Discount> GetDiscounts()
        {
            return _context.Discounts;
        }

        // GET: api/Discounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.Discounts.SingleOrDefaultAsync(x => x.Id == id);

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }

        // PUT: api/Discounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscount([FromRoute] int id, [FromBody] DiscountVM discountvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != discountvm.Id)
            {
                return BadRequest();
            }
            var discount = await _context.Discounts.FindAsync(discountvm.Id);

            discount.IsActive = discountvm.IsActive;
            discount.Name = discountvm.Name;
            discount.Percentage = discountvm.Percentage;
            discount.Month = discountvm.Mouths;
            discount.InAgreement = discountvm.InAgreement;
            discount.StartDate = discountvm.StartDate;
            discount.EndDate = discountvm.EndDate;


            _context.Entry(discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!DiscountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    SystemLog systemLog = new SystemLog();
                    systemLog.Description = e.ToMessageAndCompleteStacktrace();
                    systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                    systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                    systemLog.Parameter = JsonConvert.SerializeObject(discountvm);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                }
            }

            return NoContent();
        }

        // POST: api/Discounts
        [HttpPost]
        public async Task<IActionResult> PostDiscount([FromBody] DiscountVM discountvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Discount discount = new Discount();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //if(discountvm.InAgreement)
                    discount.IsActive = discountvm.IsActive;
                    discount.Name = discountvm.Name;
                    discount.Percentage = discountvm.Percentage;
                    discount.Month = discountvm.Mouths;
                    discount.InAgreement = discountvm.InAgreement;
                    discount.StartDate = discountvm.StartDate;
                    discount.EndDate = discountvm.EndDate;

                    _context.Discounts.Add(discount);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(discountvm);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }
            

            return CreatedAtAction("GetDiscount", new { id = discount.Id }, discount);
        }

        // DELETE: api/Discounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return Ok(discount);
        }

        [HttpPost("{idAgreement}")]
        public async Task<IActionResult> PostDiscount([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    //command.CommandText = "[dbo].[campaing_recharges]";
                    command.CommandText = "[dbo].[campaing_recharges_global]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@error",
                        DbType = DbType.String,
                        Size = 200,
                        Direction = ParameterDirection.Output
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        error = command.Parameters["@error"].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo realizar la condonación de recargos por las siguientes razones: [{error}]") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para cancelar recargos" });
            }
           
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(e => e.Id == id);
        }
    }
}