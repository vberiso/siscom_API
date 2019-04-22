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

            if (payment == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No tiene valores" });

            }

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
            var orderSale = await _context.Debts
                                         .Include(x => x.DebtDetails)
                                         .Where(x => x.Id == id)
                                         .ToListAsync();

            if (orderSale.Count == 0)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No hay orden" });

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

        [HttpPost("IncomeFromBox")]
        public async Task<IActionResult> GetIncomeFromBox([FromBody] Siscom.Agua.Api.Model.DataReportes pData)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[sp_IncomeFromBox]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaInicio", pData.FechaIni));
                command.Parameters.Add(new SqlParameter("@fechaFin", pData.FechaFin));                
                command.Parameters.Add(new SqlParameter("@CNombre", pData.CajeroNombre));
                command.Parameters.Add(new SqlParameter("@CAPaterno", pData.CajeroAPaterno));
                command.Parameters.Add(new SqlParameter("@CAMaterno", pData.CajeroAMaterno));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);
        }

        [HttpPost("IncomeByConcept")]
        public async Task<IActionResult> GetIncomeByConcept([FromBody] Siscom.Agua.Api.Model.DataReportes pData )
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[sp_IncomeByConcept]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaInicio", pData.FechaIni));
                command.Parameters.Add(new SqlParameter("@fechaFin", pData.FechaFin));                
                command.Parameters.Add(new SqlParameter("@userId", pData.CajeroId));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);            
        }

        [HttpPost("PadronWater")]
        public async Task<IActionResult> GetPadronWater([FromBody] Siscom.Agua.Api.Model.DataReportes pData)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[sp_PadronWater]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaInicio", pData.FechaIni));
                command.Parameters.Add(new SqlParameter("@fechaFin", pData.FechaFin));
                command.Parameters.Add(new SqlParameter("@FiltrarPorFecha", pData.pwaFiltrarPorContrato));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);
        }

        [HttpPost("Collection")]
        public async Task<IActionResult> GetCollection([FromBody] Siscom.Agua.Api.Model.DataReportes pData)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[sp_Collection]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaInicio", pData.FechaIni));
                command.Parameters.Add(new SqlParameter("@fechaFin", pData.FechaFin));
                command.Parameters.Add(new SqlParameter("@CNombre", pData.CajeroNombre));
                command.Parameters.Add(new SqlParameter("@CAPaterno", pData.CajeroAPaterno));
                command.Parameters.Add(new SqlParameter("@CAMaterno", pData.CajeroAMaterno));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);
        }
    }
}
