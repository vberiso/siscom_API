using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using Newtonsoft.Json;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Enums;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TransactionCancelationRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionCancelationRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCancelationRequest([FromRoute] int id)
        {
            try
            {
                Siscom.Agua.Api.Model.TransactionCancellationRequestVM tcrVM = new Model.TransactionCancellationRequestVM();
                TransactionCancellationRequest TCR = await _context.TransactionCancellationRequests.FirstOrDefaultAsync(t => t.TransactionId == id);

                tcrVM.Id = TCR.Id;
                tcrVM.Status = TCR.Status;
                tcrVM.DateRequest = TCR.DateRequest;
                tcrVM.Reason = TCR.Reason;
                tcrVM.Manager = TCR.Manager;
                tcrVM.DateAuthorization = TCR.DateAuthorization;                
                tcrVM.KeyFirebase = TCR.KeyFirebase;
                tcrVM.TransactionId = TCR.TransactionId;

                Transaction transaction = await _context.Transactions
                    .Include(tr => tr.TerminalUser)
                    .Include(tf => tf.TransactionFolios)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if(transaction != null)
                    tcrVM.Amount = transaction.Total;
                else
                    tcrVM.Amount = 0.00;

                ApplicationUser applicationUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == transaction.TerminalUser.UserId);
                if (applicationUser != null)                
                    tcrVM.UserName = string.Format("{0} {1} {2}", applicationUser.UserName, applicationUser.LastName, applicationUser.SecondLastName);
                else
                    tcrVM.UserName = "Desconocido";
                
                Payment payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionFolio == transaction.Folio);
                if (payment != null)
                    tcrVM.BranchOffice = payment.BranchOffice;
                else
                    tcrVM.BranchOffice = "Sin Determinar";

                if (transaction.TransactionFolios != null)
                    tcrVM.PrintingFolio = applicationUser.Serial + "-" +  transaction.TransactionFolios.FirstOrDefault().Folio.Remove(0,1);
                else
                    tcrVM.PrintingFolio = "----------";

                return Ok(tcrVM);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No se pudo obtener el objeto TransactionCancelationRequest." });
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCancelationRequest([FromRoute] int id)
        {
            try
            {
                return Ok(_context.TransactionCancellationRequests);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No se pudo obtener el objeto TransactionCancelationRequest." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostCancelationRequest([FromBody] TransactionCancellationRequest pTCR)
        {          
            try
            {
                if ( pTCR.TransactionId == 0 || string.IsNullOrEmpty(pTCR.Reason) || pTCR.DateRequest == null )
                {
                    return StatusCode((int)TypeError.Code.PartialContent, new { Error = "Faltan elementos al objeto." });
                }

                //Solicitud de cancelacion
                TransactionCancellationRequest TCR = new TransactionCancellationRequest();
                _context.TransactionCancellationRequests.Add(pTCR);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.Message;
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pTCR);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para crear la solicitud." });
            }

            return Ok(pTCR);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] TransactionCancellationRequest TCR)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != TCR.Id)
            {
                return BadRequest();
            }

            try
            {                
                _context.Entry(TCR).State = EntityState.Modified;
                await _context.SaveChangesAsync();                
                return Ok();                
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.Message;
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(TCR);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar la solicitud de cancelación" });
            }
        }


    }
}