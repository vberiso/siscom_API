using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model.SOSAPAC;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    public class CutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CutsController(ApplicationDbContext context)
        {

            _context = context;
        }
        // GET: api/Suburbs
        [HttpGet("Suburbs")]
        public async Task<IActionResult> GetSuburbs()
        {
            //var Agreement = await _context.Agreements
            //                        .Include(s => s.Addresses)
            //                            .ThenInclude(su => su.Suburbs)
            //                        .Include(d => d.Debts)
            //                        .Where(x => x.Debts.Any(gs => _context.Statuses
            //                                                              .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && 
            //                                                              (gs.Amount - gs.OnAccount) > 1000))
            //                        .ToListAsync();
            var Agreement = await _context.Agreements
                                            .Include(x => x.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Where(x => x.TypeStateServiceId == 1 && x.Addresses.Any(z => z.TypeAddress == "DIR01"))
                                            .ToListAsync();

            List<CutSuburbVM> suburbs = new List<CutSuburbVM>();
            Agreement.ForEach(x =>
            {
                x.Addresses.ToList().ForEach(z =>
                {
                    suburbs.Add(new CutSuburbVM
                    {
                        Id = z.Suburbs.Id,
                        Name = z.Suburbs.Name,
                        Route = x.Route
                    });
                });
            });


            //List<Suburb> 
            return Ok(suburbs.GroupBy(x => x.Name)
                             .Select(g => g.First())
                             .OrderBy(x => x.Name)
                             .ToList());

        }

        // GET: api/Suburbs/2/5000/
        [HttpGet("{NumOfPeriods}/{Debit}/{Suburbs}")]
        public async Task<IActionResult> GetSuburbs([FromRoute] int NumOfPeriods, [FromRoute] decimal Debit, [FromRoute] string Suburbs)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementBySuburbs]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Suburbs",
                        DbType = DbType.String,
                        Value = Suburbs
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@NumPeriods",
                        DbType = DbType.Int32,
                        Value = NumOfPeriods
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Amount",
                        DbType = DbType.Decimal,
                        Value = Debit
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(result);
                    }
                }
                return Ok(dataTable);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            
        }

        [HttpPost("{Agreements}")]
        public async Task<IActionResult> PostNotifications([FromRoute] string Agreements)
        {
            List<string> agreements = new List<string>(Agreements.Split(","));
            List<string> error = new List<string>();
            try
            {
                foreach (var item in Agreements.Split(","))
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "[dbo].[generate_notification]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@id_agreement",
                            DbType = DbType.Int32,
                            Value = Convert.ToInt32(item)
                        });

                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@error",
                            DbType = DbType.String,
                            Size = 200,
                            Direction = ParameterDirection.Output
                        });

                        this._context.Database.OpenConnection();
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            if (!string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()))
                                error.Add("El contrado (ID: " + item + ") -> " + command.Parameters["@error"].Value.ToString());
                        }
                    }
                }

                if(error.Count > 0)
                {
                    if(agreements.Count != error.Count)
                    {
                        return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format($"Se completaron {agreements.Count - error.Count} de {agreements.Count} de el proceso de notificación pero no se genero notificacion para las siguientes cuentas ID: [{string.Join(Environment.NewLine, error)}]") });
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format($"No se pudo realizar el proceso de descuentos por las siguientes razones: [{string.Join(Environment.NewLine, error)}]") });
                    }

                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = $"Agreement List: {Agreements}";
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción de notificaciones" });
            }
           
        }

        private static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
    }
}