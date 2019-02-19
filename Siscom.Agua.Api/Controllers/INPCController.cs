using System;
using System.Collections.Generic;
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
    public class INPCController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public INPCController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/INPC
        [HttpGet]
        public IEnumerable<INPC> GetINPCs()
        {
            return _context.INPCs;
        }

        // GET: api/INPC/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetINPC([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var iNPC = await _context.INPCs.FindAsync(id);

            if (iNPC == null)
            {
                return NotFound();
            }

            return Ok(iNPC);
        }

        // PUT: api/INPC/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutINPC([FromRoute] int id, [FromBody] INPC iNPC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != iNPC.Id)
            {
                return BadRequest();
            }

            if (!INPCExists(id))
            {
                return NotFound();
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _context.Entry(iNPC).State = EntityState.Modified;
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
                systemLog.Parameter = JsonConvert.SerializeObject(iNPC);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el INPC" });
            }

            return NoContent();
        }

        // POST: api/INPC
        [HttpPost]
        public async Task<IActionResult> PostINPC([FromBody] INPC iNPC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _context.INPCs.Add(iNPC);
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
                systemLog.Parameter = JsonConvert.SerializeObject(iNPC);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el INPC" });
            }
          

            return CreatedAtAction("GetINPC", new { id = iNPC.Id }, iNPC);
        }

        private bool INPCExists(int id)
        {
            return _context.INPCs.Any(e => e.Id == id);
        }
    }
}