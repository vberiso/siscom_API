using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class DebtsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DebtsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Debts/5
        [HttpGet("{idAgreement}")]
        public async Task<IActionResult> GetDebt([FromRoute] int idAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var debt = await _context.Debts.Include(dd => dd.DebtDetails)
                        .Where(gs => _context.Statuses
                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == idAgreement).OrderBy(x => x.DebitDate).ToListAsync(); 

            if (debt == null)
            {
                return NotFound();
            }

            return Ok(debt);
        }


       
    }
}