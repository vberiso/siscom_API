using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

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

        [HttpGet("DiscountAnnual/{idAgreement}")]
        public async Task<IActionResult> AssignDiscountAnnual([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            List<BillingYear> entities = null;
            bool IsValid = false;

            //Get informaion from this agreement in the type of intake 
            var agreement = await _context.Agreements.Include(x => x.TypeIntake)
                                          .Where (z => z.Id == idAgreement)
                                          .FirstOrDefaultAsync();

            //Check if not exit debt for this agreement
            var debt = await _context.Debts.Include(dd => dd.DebtDetails)
                                     .Where(gs => gs.AgreementId == idAgreement)
                                     .OrderBy(x => x.DebitDate)
                                     .ToListAsync();

            if (agreement.TypeIntake.Acronym != "HA")
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Las características del contrato no cuenta con lo necesario para asignar un descuento: [Tipo de Toma no es Habitacional]"});
            }
           
            if (debt != null)
            {
                var a = debt.Where(gs => _context.Statuses
                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status)).ToList();
                if (a.Count > 0)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Las características del contrato no cuenta con lo necesario para asignar un descuento: [El contrato tiene adeudos]" });
                var b = debt.Where(t => t.Type == "TIP01" && t.Year == 2018 && t.DebtPeriodId == 12).ToList();
                if (b.Count > 0)
                    IsValid = true;
                else
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Las características del contrato no cuenta con lo necesario para asignar un descuento: [El contrato no tiene facturado el ultimo periodo]" });

            }
            if (IsValid)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "billing_year";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
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
                        if (!result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        else
                        {
                            entities = new List<BillingYear>();
                            while (result.Read())
                            {
                                var bl = new BillingYear();
                                bl.conde_concept = result[0].ToString();
                                bl.name_concept = result[1].ToString();
                                bl.have_tax = Convert.ToBoolean(result[3]);
                                bl.amount = Convert.ToDecimal(result[2].ToString());
                                entities.Add(bl);
                            }
                        }

                    }
                }
            }

           
            if(entities != null)
            {
                return Ok(entities);
            }
            else
            {
                return Ok(error);
            }
        }

        private List<T> RawSqlStoreProcedure<T>(string query, Func<DbDataReader, T> map)
        {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    _context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        var entities = new List<T>();

                        while (result.Read())
                        {
                            entities.Add(map(result));
                        }

                        return entities;
                    }
                }
        }
    }
}