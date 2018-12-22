﻿using System;
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
    public class PaymentHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentsByAgreement([FromRoute] int id)
        {
            var payments = await _context.Payments
                                         .Where(a => a.AgreementId == id)
                                         .OrderByDescending(f => f.Status)
                                         .ToListAsync();

            return new ObjectResult(payments);
        }

        [HttpGet("{debtId}")]
        public async Task<IActionResult> GetPaymentsByDebt([FromRoute] int debtId)
        {
            var debts = await _context.Debts
                                         .Include(dd => dd.DebtDetails)
                                         .Where(d => d.Id == debtId)
                                         .OrderByDescending(f => f.Status)
                                         .ToListAsync();

            return new ObjectResult(debts);
        }
    }
}