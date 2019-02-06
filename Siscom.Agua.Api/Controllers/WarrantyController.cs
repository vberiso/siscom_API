using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Warranty/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class WarrantyController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public WarrantyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Warranty
        [HttpGet]
        public IEnumerable<Warranty> GetWarranties()
        {
            return _context.Warranties;
        }
    }
}
