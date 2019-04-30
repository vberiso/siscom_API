using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
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
                .Include(p =>p.TaxReceipts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
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
                                        .FirstOrDefaultAsync(m => m.TransactionFolio== folio);
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
    }
}
