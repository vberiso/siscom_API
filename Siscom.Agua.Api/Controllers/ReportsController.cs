﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Reports/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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

                .Where(p => p.PaymentDate.Year == Convert.ToDateTime(Date).Year).ToListAsync();


            var debt = _context.DebtDiscounts.Where(d => d.DebtId == 450620).ToList().Sum(d => d.DiscountAmount);




            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // GET: api/Prueba
        [HttpGet("Prueba/{Date}")]
        public async Task<IActionResult> GetReportDebt([FromRoute] string Date)
        {
            var payment = await _context.Payments.Include(x => x.PaymentDetails)
                        .Where(p => p.PaymentDate.Year == Convert.ToDateTime(Date).Year).FirstOrDefaultAsync();


            payment.PaymentDetails.ToList().ForEach(x =>
            {

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

        [HttpGet("Consulta/{Num}")]
        public async Task<IActionResult> GetConsult([FromRoute] string Num)
        {


            var cons = await _context.Agreements
                            .Include(p => p.Addresses)
                            .Include(c => c.Clients)
                            .Include(t => t.TypeService)
                            .Where(d => d.Account == Num).ToListAsync();

            if (cons == null)
            {
                return NotFound();
            }
            return Ok(cons);
        }

        [HttpGet("ExerciseMonth/{Date}")]
        public async Task<IActionResult> GetExerciseMonth([FromRoute] string Date)
        {

            Model.Report mon = new Model.Report();
            decimal total = 0;
            decimal subto = 0;

            var payment = await _context.Payments
                .Where(p => p.PaymentDate.Month == Convert.ToDateTime(Date).Month).ToListAsync();

            total = payment.Sum(x => x.Total);
            subto = payment.Sum(s => s.Subtotal);

            if(total == 0){
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No existen pagos en este mes" });

            }

            mon.Total = total;
            mon.subTotal = subto; 


            if (mon == null){
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No encontrado" });

                //return NotFound();
            }
            return Ok(mon);
        }

        [HttpGet("Historic/{Date}")]
        public async Task<IActionResult> GetHistoric([FromRoute] string Date)
        {

            Model.Report ps = new Model.Report();
            decimal result = 0;
            decimal sub = 0;


            var payment = await _context.Payments
                .Where(p => p.PaymentDate.Year == Convert.ToDateTime(Date).Year).ToListAsync();




            result = payment.Sum(x => x.Total);
            sub = payment.Sum(s => s.Subtotal );

            if(result == 0){

                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No hay registro" });

            }

            ps.Total = result;
            ps.subTotal = sub;


            if (ps == null){
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No encontrado" });

                //return NotFound();
            }
            return Ok(ps);

            //return Ok("Total:"+result);
        }


    }
}