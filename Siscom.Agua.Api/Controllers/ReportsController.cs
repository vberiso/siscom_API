﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Reports/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase 
    {

        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Report
        [HttpGet("{Date}")]
        public async Task<IActionResult> GetReports([FromRoute] string Date)
        {


            var payment = await _context.Payments
                            .Include(p => p.PaymentDetails)

                .Where(p=> p.PaymentDate.Year == Convert.ToDateTime(Date).Year).ToListAsync();


            var debt = _context.DebtDiscounts.Where(d => d.DebtId == 450620).ToList().Sum(d => d.DiscountAmount);

           
            

            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // GET: api/Agreements
        [HttpGet]
        public async Task<IActionResult> GetReportDebt()
        {
            var payment = await _context.Payments.Include(x => x.PaymentDetails).FirstOrDefaultAsync();


            payment.PaymentDetails.ToList().ForEach(x =>{

                x.Debt = _context.Debts
                    .Include(dd => dd.DebtDiscounts)
                    .Where(i => i.Id == x.DebtId)
                    .FirstOrDefault();

            });
            //payment..ToList().ForEach(x =>
            //{
            //    x.Suburbs = _context.Suburbs.Include(r => r.Regions)
            //                                .Include(c => c.Clasifications)
            //                                .Include(t => t.Towns)
            //                                    .ThenInclude(s => s.States)
            //                                    .ThenInclude(c => c.Countries)
            //                                .Where(i => i.Id == x.Suburbs.Id)
            //                                .SingleOrDefault();
            //});
            //var debtDis = await _context.DebtDiscounts.ToListAsync();
            //var payment = await _context.PaymentDetails.Where(id => id.DebtId == 118313).ToListAsync();


           
            return Ok(payment);
        }

    }
}
