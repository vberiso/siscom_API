using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points Payments
    /// </summary>
    [Route("api/Payments")]
    [Produces("application/json")]
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }
    }
}
