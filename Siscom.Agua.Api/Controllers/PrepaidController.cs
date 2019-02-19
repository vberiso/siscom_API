using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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
            var description = await _context.Statuses.Where(x => x.GroupStatusId == 5).ToListAsync();

            prepaid.ForEach(x =>
            {
                x.StatusDescription = (from p in description
                                       where p.CodeName == x.Status
                                       select p).FirstOrDefault().Description;
            });

            return new ObjectResult(prepaid);
        }

    }
}