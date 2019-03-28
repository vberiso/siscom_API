using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        [HttpGet("ExerciseMonth/{DateYear}/{Month}")]
        public async Task<IActionResult> GetExerciseMonth([FromRoute] string DateYear, string Month)
        {

            Model.Report mon = new Model.Report();
            decimal total = 0;
            decimal subto = 0;

            var payment = await _context.Payments
                                .Where(p => p.PaymentDate.Year == Convert.ToDateTime(DateYear).Year && 
                                     Convert.ToDateTime(Month).Month == p.PaymentDate.Month).ToListAsync();


            //var payment = first
                //.Where().ToListAsync();

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
        [HttpGet("GetOrderSaleByFolio/{id}")]
        public async Task<IActionResult> GetOrderSaleByFolio([FromRoute]int id)
        {
            var orderSale = await _context.OrderSales
                                         .Include(x => x.OrderSaleDetails)
                                         .Include(x => x.TaxUser)
                                           .ThenInclude(user => user.TaxAddresses)
                                         .Where(x => x.Id == id)
                                         .ToListAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

            return Ok(orderSale);
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

        [HttpGet("IncomeTransactions/{dateInicio}/{dateFin}/{paymentStatus}")]
        public async Task<IActionResult> GetIncomeTransactions([FromRoute] string dateInicio, string dateFin, string paymentStatus)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[sp_IngresosDeCaja]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaInicio", dateInicio));
                command.Parameters.Add(new SqlParameter("@fechaFin", dateFin));
                command.Parameters.Add(new SqlParameter("@paymentStatus", paymentStatus));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);

            //var tmp2 = (from p in _context.Payments
            //          join a in _context.Agreements on p.AgreementId equals a.Id
            //          where p.Status == "EP001" && p.PaymentDate >= FechaInicio && p.PaymentDate < FechaFin
            //          select new { fecha_pago = p.PaymentDate, a.Account, p.Total, p.BranchOffice })
            //          .ToList();

            //var tmp4 = (from p in _context.Payments
            //            join a in _context.Agreements on p.AgreementId equals a.Id
            //            join py in _context.PayMethods on p.PayMethodId equals py.Id
            //            join o in _context.OriginPayments on p.OriginPaymentId equals o.Id
            //            join e in _context.ExternalOriginPayments on p.ExternalOriginPaymentId equals e.Id
            //            join c in _context.Clients on a.Id equals c.AgreementId
            //            join t in _context.Transactions on p.TransactionFolio equals t.Folio
            //            join tu in _context.TerminalUsers on t.TerminalUserId equals tu.Id
            //            join u in _context.Users on tu.UserId equals u.Id
            //            join f in _context.TransactionFolios on t.Id equals f.TransactionId
            //            where p.Status == "EP001" && p.PaymentDate >= FechaInicio && p.PaymentDate < FechaFin
            //            select new
            //            {
            //                fecha_pago = p.PaymentDate,
            //                a.Account,
            //                p.Total,
            //                p.BranchOffice,
            //                metodo_pago = py.Name,
            //                origen_pago = o.Name,
            //                banco = e.Name,
            //                folio = p.TransactionFolio,
            //                folio_impresion = f.Folio,
            //                cajero = string.Format("{0} {1} {2}", u.Name, u.UserName ,u.SecondLastName),
            //                cliente = string.Format("{0} {1} {2}", c.Name, c.LastName, c.SecondLastName)
            //            })
            //          .ToList();

        }

        [HttpGet("IncomeTransactions")]
        public async Task<IActionResult> GetIncomeByConcept()
        {
            string error = string.Empty;
            //var dataTable = new DataTable();
            //using (var command = _context.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = "[dbo].[sp_IngresosDeCaja]";
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.Parameters.Add(new SqlParameter("@fechaInicio", dateInicio));
            //    command.Parameters.Add(new SqlParameter("@fechaFin", dateFin));
            //    command.Parameters.Add(new SqlParameter("@paymentStatus", paymentStatus));

            //    this._context.Database.OpenConnection();
            //    using (var result = await command.ExecuteReaderAsync())
            //    {
            //        dataTable.Load(result);
            //    }
            //}
            //return Ok(dataTable);
            return Ok();
        }


    }
}
