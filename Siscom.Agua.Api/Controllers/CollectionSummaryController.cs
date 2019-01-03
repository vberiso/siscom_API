using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/TerminalUser")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class CollectionSummaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CollectionSummaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] CollectionSummaryVM summaryVM)
        {
            var payments = await _context.Payments
                                         .Include(x => x.PaymentDetails)
                                         .Where(d => d.PaymentDate.Date == summaryVM.StarDate)
                                         .ToListAsync();
        }
    }
}