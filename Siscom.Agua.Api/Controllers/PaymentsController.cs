using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// <summary>
    /// End Points Payments
    /// </summary>
    [Route("api/Payments")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Payments
        /// </summary>
        /// <returns></returns>
        // GET: api/Payments
        [HttpGet]
        public IEnumerable<Payment> GetPayment()
        {
            return _context.Payments;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Payments which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>BranchOffice Model</returns>
        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment([FromRoute] int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.ExternalOriginPayment)
                .Include(p => p.OriginPayment)
                .Include(p => p.PayMethod)
                .Include(p => p.PaymentDetails)
                .Include(p => p.TaxReceipts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        /// <summary>
        /// Obtendo los pagos a partir del id iddebt.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("fromDebt/{idDebt}")]
        public async Task<IActionResult> GetPaymentsFromDebt([FromRoute] int idDebt)
        {
            if (idDebt == 0)
            {
                return NotFound();
            }

            var idsPayment = _context.PaymentDetails.Where(x => x.DebtId == idDebt).Select(y => y.PaymentId).Distinct().ToList();
            if (idsPayment == null || idsPayment.Count == 0)
                return NoContent();
            
            var payment = await _context.Payments
                .Include(p => p.ExternalOriginPayment)
                .Include(p => p.OriginPayment)
                .Include(p => p.PayMethod)
                .Include(p => p.PaymentDetails)
                .Include(p => p.TaxReceipts)
                .Where(m => idsPayment.Contains(m.Id) && m.HaveTaxReceipt == true).ToListAsync();

            if (payment == null)
            {
                return NoContent();
            }

            return Ok(payment);
        }

        /// <summary>
        /// This will provide details for the specific ID, of Payments which is being passed
        /// </summary>
        /// <param name="folio">Mandatory</param>
        /// <returns>BranchOffice Model</returns>
        // GET: api/Payments/5
        [HttpGet("folio/{folio}")]
        public async Task<IActionResult> GetPaymentFolio([FromRoute] string folio)
        {
            if (String.IsNullOrEmpty(folio))
            {
                return NotFound();
            }

            var payment = await _context.Payments
                                        .Include(p => p.ExternalOriginPayment)
                                        .Include(p => p.OriginPayment)
                                        .Include(p => p.PayMethod)
                                        .Include(p => p.PaymentDetails)
                                        .Include(p => p.TaxReceipts)
                                        .FirstOrDefaultAsync(m => m.TransactionFolio== folio);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }
        [HttpGet("TaxReceipt/{Account}")]
        public async Task<IActionResult> GetPaymentByAccount([FromRoute] string Account)
        {
            if (String.IsNullOrEmpty(Account))
            {
                return NotFound();
            }

            var payment = await _context.Payments
                                        .Include(p => p.ExternalOriginPayment)
                                        .Include(p => p.OriginPayment)
                                        .Include(p => p.PayMethod)
                                        .Include(p => p.PaymentDetails)
                                        .Include(p => p.TaxReceipts)
                                        .FirstOrDefaultAsync(m => m.Account == Account);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpGet("Resume/{folio}")]
        public async Task<IActionResult> GetPaymentResume([FromRoute] string folio)
        {
            if (String.IsNullOrEmpty(folio))
            {
                return NotFound();
            }

            PaymentResume paymentResume = new PaymentResume();
            paymentResume.payment = await _context.Payments
                                        .Include(p => p.ExternalOriginPayment)
                                        .Include(p => p.OriginPayment)
                                        .Include(p => p.PayMethod)
                                        .Include(p => p.PaymentDetails)
                                        .FirstOrDefaultAsync(m => m.Account == folio);
            if (paymentResume.payment == null)
            {
                return NotFound();
            }

            paymentResume.orderSale = await _context.OrderSales
                                        .Include(os => os.OrderSaleDetails)
                                        .Include(os => os.OrderSaleDiscounts)
                                        .Include(os => os.TaxUser)
                                        .ThenInclude(os => os.TaxAddresses)
                                        .FirstOrDefaultAsync(x => x.Id == paymentResume.payment.OrderSaleId);
                                        

            return Ok(paymentResume);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payment.Id)
            {
                return BadRequest();
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _context.Entry(payment).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    scope.Complete();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(payment);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el pago" });
            }
        }

        [HttpPost("SistemaAdministracionContable/{idPayment}")]
        public async Task<IActionResult> PostSAC([FromRoute] int idPayment)
        {
            string error = string.Empty;
            try
            {
                var payments = _context.AccountingPayments.Where(x => x.PaymentId == idPayment && x.Status == "CB0003").OrderBy(x => x.Secuential).ToList();
                foreach (var item in payments)
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "[dbo].[AccountingSAC]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@idAccountingPayment", item.Id));
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@Error",
                            DbType = DbType.String,
                            Size = 200,
                            Direction = ParameterDirection.Output
                        });
                        this._context.Database.OpenConnection();
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            error += !string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()) ? command.Parameters["@error"].Value.ToString() + " -- " : "";
                        }
                    }
                }
                if (string.IsNullOrEmpty(error))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = error });
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = "Error al ejecutar el SP_SAC con el id de pago:" + idPayment;
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

        }
    }
}
