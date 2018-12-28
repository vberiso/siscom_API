using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PrepaidController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrepaidController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{AgreementId}")]
        public async Task<IActionResult> GetPrepaidDetails([FromRoute] int AgreementId)
        {
            var prepaid = await _context.Prepaids
                                        .Include(x => x.PrepaidDetails)
                                        .ThenInclude(x => x.DebtPrepaids)
                                        .Where(i => i.AgreementId == AgreementId)
                                        .ToListAsync();

            return new ObjectResult(prepaid);
        }

    }
}