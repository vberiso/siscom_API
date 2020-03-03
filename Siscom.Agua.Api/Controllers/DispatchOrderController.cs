using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DispatchOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DispatchOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getAgremmentsOfStaff([FromRoute] int id)
        {
            var dispatchOrder = _context.DispatchOrders.FirstOrDefaultAsync(x => x.Id == id);
            return StatusCode(StatusCodes.Status200OK, dispatchOrder);
        }

        [HttpPost()]
        public async Task<IActionResult> PostDispatchOrder([FromBody] DispatchOrder dispatchOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {   
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dispatchOrder1 = await _context.DispatchOrders.Where(d => d.OrderWorkId == dispatchOrder.OrderWorkId).FirstOrDefaultAsync();

                    if(dispatchOrder1 == null)
                    {
                        _context.DispatchOrders.Add(dispatchOrder);
                        await _context.SaveChangesAsync();
                        scope.Complete();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK, new { msg = "Ya existe un despacho ligado a esta order work" });
                    }                    
                }
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(dispatchOrder);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode(StatusCodes.Status400BadRequest , new { Error = "Problemas para agregar un dispatch_order" });
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> PutDispatchOrder([FromRoute] int id, [FromBody] DispatchOrder dispatchOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != dispatchOrder.Id)
                    {
                        return BadRequest();
                    }
                    _context.Entry(dispatchOrder).State = EntityState.Modified;                    
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
                return StatusCode(StatusCodes.Status200OK, new { msg = "Actualización exitosa" }  );
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(dispatchOrder);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar un dispatch_order" });
            }
        }

    }
}