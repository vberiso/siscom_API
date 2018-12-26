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
    public class DiscountValidatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscountValidatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idAgreement}")]
        public IActionResult AssignDiscount([FromRoute] int idAgreement)
        {
            return Ok(_context.Database.ExecuteSqlCommand("billing_year @p0", parameters: new[] { idAgreement }));
        }
    }
}