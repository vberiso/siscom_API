﻿using System;
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
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CollectionSummaryVM summaryVM)
        {
            var payments = await _context.Payments
                                         .Include(x => x.PaymentDetails)
                                         .Include(op => op.OriginPayment)
                                         .Include(eop => eop.ExternalOriginPayment)
                                         .Include(pm => pm.PayMethod)
                                         .Where(d => d.PaymentDate.Date >= summaryVM.StarDate.Date && d.PaymentDate.Date <= summaryVM.EndDate.Date)
                                         .OrderBy(x => x.Status)
                                         .ToListAsync();
            return Ok(payments);
        }
    }
}