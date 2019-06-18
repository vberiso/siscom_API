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
                if(elem != null)
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
    }
}