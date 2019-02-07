using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/BreachDetail/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class BreachDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BreachDetailController(ApplicationDbContext context)
        {
            _context = context;

        }
        // GET: api/BreachDetail
        [HttpGet]
        public IEnumerable<BreachDetail> GetBreachDetails()
        {
            return _context.BreachDetails;
        }
    }
}
