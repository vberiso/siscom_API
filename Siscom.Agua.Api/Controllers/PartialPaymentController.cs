using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class PartialPaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;

        public PartialPaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpPost("billingPartialPayment/{idAgreement}")]

        public async Task<IActionResult> PostBillingPartialPayment([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[billing_partial_payment]";
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
                        if (result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        error = command.Parameters["@error"].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return Ok("Se liberó la cuota para cobro");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo liberar la cuota para cobro: [{error}]") });


                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para liberar cuota para cobro" });
            }

        }

        [HttpPost("cancelPartialPayment/{idAgreement}")]

        public async Task<IActionResult> PostCancelPartialPayment([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[cancel_partial_payment]";
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
                        if (result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        error = command.Parameters["@error"].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return Ok("Se canceló el convenio");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo cancelar el convenio:  [{error}]") });


                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para cancelar el convenio" });
            }

        }

        [HttpPost("createPartialPayment")]
        public async Task<IActionResult> PostPartialPaymentBody([FromBody] WebPartialPayment partial)
        {
            string error = string.Empty;
            string folio = string.Empty;

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[generate_partial_payment]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", partial.idAgreement));
                    command.Parameters.Add(new SqlParameter("@number_of_payments", partial.numberPayments));
                    command.Parameters.Add(new SqlParameter("@observations", partial.observations));
                    command.Parameters.Add(new SqlParameter("@signature_name", partial.signatureName));
                    command.Parameters.Add(new SqlParameter("@identification_number", partial.idNumber));
                    command.Parameters.Add(new SqlParameter("@identification_card", partial.idCard));
                    command.Parameters.Add(new SqlParameter("@email", partial.email));
                    command.Parameters.Add(new SqlParameter("@phone", partial.phone));
                    command.Parameters.Add(new SqlParameter("@release_days", partial.releaseDay));
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@error",
                        DbType = DbType.String,
                        Size = 150,
                        Direction = ParameterDirection.Output
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@folio",
                        DbType = DbType.String,
                        Size = 30,
                        Direction = ParameterDirection.Output
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        error = command.Parameters["@error"].Value.ToString();
                        folio = command.Parameters["@folio"].Value.ToString();

                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return Ok(folio);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo generar el convenio:{error}") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = partial.idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para generar convenio" });
            }

        }

        [HttpGet("FindPartialPaymentAgreement/{account}/{idAgreement}")]
        public async Task<IActionResult> FindPartialPaymentAgreement([FromRoute] string account, [FromRoute] int idAgreement)
        {
            List<PartialPaymentAgreement> partial = new List<PartialPaymentAgreement>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        if (account != "false" && idAgreement == 0)
                        {
                            command.CommandText = "select p.id_partial_payment , " +
                            "p.folio, convert(varchar,p.partial_payment_date, 103) partial_payment_date , " +
                            "p.amount , p.number_of_payments , p.AgreementId," +
                            "s.[description], p.expiration_date, a.account from partial_payment p , " +
                            "Status s, Agreement a where p.AgreementId = a.id_agreement and s.id_status = p.status and a.account = '" + account + "'";
                        }
                        if (idAgreement != 0 && account == "false")
                        {
                            command.CommandText = "select p.id_partial_payment , " +
                            "p.folio, convert(varchar,p.partial_payment_date, 103) partial_payment_date , " +
                            "p.amount , p.number_of_payments , p.AgreementId," +
                            "s.[description], p.expiration_date, a.account from partial_payment p , " +
                            "Status s, Agreement a where p.AgreementId = '" + idAgreement + "' and s.id_status = p.status and a.id_agreement = p.AgreementId";
                        }
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentAgreement
                                {
                                    idPartialPayment = Convert.ToInt32(result[0]),
                                    folio = result[1].ToString(),
                                    partialPaymentDate = result[2].ToString(),
                                    amount = Convert.ToDecimal(result[3]),
                                    numberPayments = Convert.ToInt32(result[4]),
                                    AgreementId = Convert.ToInt32(result[5]),
                                    description = result[6].ToString(),
                                    expiration_date = result[7].ToString(),
                                    Account = Convert.ToInt32(result[8])
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = account;
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }

        [HttpGet("FindPartialPaymentFolio/{folio}")]
        public async Task<IActionResult> FindPartialPaymentFolio([FromRoute] string folio)
        {
            List<PartialPaymentAgreement> partial = new List<PartialPaymentAgreement>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select p.id_partial_payment , " +
                            "p.folio, convert(varchar,p.partial_payment_date, 103) partial_payment_date , " +
                            "p.amount , p.number_of_payments, p.AgreementId," +
                            "s.[description], p.expiration_date, agr.account from partial_payment p , " +
                            "Status s, Agreement agr where p.folio = '" + folio + "' and s.id_status = p.status and agr.id_agreement = p.AgreementId";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentAgreement
                                {
                                    idPartialPayment = Convert.ToInt32(result[0]),
                                    folio = result[1].ToString(),
                                    partialPaymentDate = result[2].ToString(),
                                    amount = Convert.ToDecimal(result[3]),
                                    numberPayments = Convert.ToInt32(result[4]),
                                    AgreementId = Convert.ToInt32(result[5]),
                                    description = result[6].ToString(),
                                    expiration_date = result[7].ToString(),
                                    Account = Convert.ToInt32(result[8])
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = folio.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }

        [HttpGet("FindPartialPaymentAmount/{PartialPaymentId}")]
        public async Task<IActionResult> FindPartialPaymentAmount([FromRoute] int PartialPaymentId)
        {
            List<PartialPaymentAmount> partial = new List<PartialPaymentAmount>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select pp.payment_number, " +
                            "pp.amount, pp.on_account, s.[description], " +
                            "convert(varchar,pp.relase_date, 103) relase_date, " +
                            "convert(varchar,pp.payment_date, 103) payment_date, "+
                            "convert(varchar, pp.release_period, 103), release_period " +
                            "from partial_payment_detail pp , " +
                            "Status s where pp.PartialPaymentId = '" + PartialPaymentId + "' " +
                            "and s.id_status = pp.status";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentAmount
                                {
                                    paymentNumber = Convert.ToInt32(result[0]),
                                    amount = Convert.ToDecimal(result[1]),
                                    onAccount = Convert.ToDecimal(result[2]),
                                    description = result[3].ToString(),
                                    releaseDate = result[4].ToString(),
                                    paymentDay = result[5].ToString(),
                                    releasePeriod = result[6].ToString()
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = PartialPaymentId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }

        [HttpGet("FindPartialPaymentConcepts/{PartialPaymentId}")]
        public async Task<IActionResult> FindPartialPaymentConcepts([FromRoute] int PartialPaymentId)
        {
            List<PartialPaymentConcept> partial = new List<PartialPaymentConcept>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select ppdc.name_concept, " +
                            "sum(ppdc.amount) amount from partial_payment_detail ppd , " +
                            "Partial_Payment_Detail_Concept ppdc where ppd.PartialPaymentId = '" + PartialPaymentId + "'  " +
                            " and ppdc.PartialPaymentDetailId = ppd.id_partial_payment_detail " +
                            " group by ppdc.name_concept";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentConcept
                                {
                                    nameConcept = result[0].ToString(),
                                    amount = Convert.ToDecimal(result[1]),
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = PartialPaymentId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }

        [HttpGet("FindPartialPaymentReceipts/{PartialPaymentId}")]
        public async Task<IActionResult> FindPartialPaymentReceipts([FromRoute] int PartialPaymentId)
        {
            List<PartialPaymentReceipts> partial = new List<PartialPaymentReceipts>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select (select t.[description] from [Type] t where t.id_type = d.[type]) [type], (d.amount - d.on_account) amount," +
                            " d.from_date, " +
                            "d.until_date from partial_payment p, " +
                            "Partial_Payment_Debt pd, " +
                            "Debt d where p.id_partial_payment = '" + PartialPaymentId + "' and pd.PartialPaymentId = p.id_partial_payment  " +
                            "and d.id_debt = pd.DebtId";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentReceipts
                                {
                                    type = result[0].ToString(),
                                    amount = Convert.ToDecimal(result[1]),
                                    fromDate = result[2].ToString(),
                                    untilDate = result[3].ToString()
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudieron buscar los recibos convenidos") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = PartialPaymentId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar los recibos convenidos" });
            }
        }

        [HttpGet("FindPartialPaymentToAgree/{idAgreement}")]
        public async Task<IActionResult> FindPartialPaymentToAgree([FromRoute] int idAgreement)
        {
            List<PartialPaymentAgree> partial = new List<PartialPaymentAgree>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select (select t.[description] from [Type] t where t.id_type = d.[type]) [type]," +
                            " d.from_date," +
                            " d.until_date," +
                            " (dd.amount - dd.on_account) amount," +
                            " d.[year]," +
                            " d.DebtPeriodId," +
                            " d.AgreementId," +
                            " dd.have_tax," +
                            " dd.name_concept " +
                            "from Debt d " +
                            "inner join Debt_Detail dd on d.id_debt = dd.DebtId " +
                            "where d.AgreementId = '" + idAgreement + "' " +
                            "and d.[status] in ('ED001','ED004','ED007','ED011')" +
                            "and d.[type] <> 'TIP06'";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentAgree
                                {
                                    type = result[0].ToString(),
                                    fromDate = result[1].ToString(),
                                    untilDate = result[2].ToString(),
                                    amount = Convert.ToDecimal(result[3]),
                                    year = Convert.ToInt32(result[4]),
                                    debtPeriodId = Convert.ToInt32(result[5]),
                                    AgreementId = Convert.ToInt32(result[6]),
                                    haveTax = Convert.ToBoolean(result[7]),
                                    nameConcept = result[8].ToString()
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }

        [HttpGet("FindPartialPaymentDebt/{PartialPaymentId}")]
        public async Task<IActionResult> FindPartialPaymentDebt([FromRoute] int PartialPaymentId)
        {
            List<PartialPaymentDebts> partial = new List<PartialPaymentDebts>();

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "select sum(r.amount) amount, r.name_concept, sum(r.iva) iva from(select(b.[amount]) amount, b.name_concept, b.have_tax," +
                            " case when b.have_tax = 1 then b.amount * 0.16 else 0 end iva from[dbo].[partial_payment_detail] a, [dbo].[Partial_Payment_Detail_Concept] b " +
                            "where PartialPaymentId = '" + PartialPaymentId + "' and b.PartialPaymentDetailId = a.id_partial_payment_detail) r group by r.name_concept";

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                partial.Add(new PartialPaymentDebts
                                {
                                    amount = Convert.ToDecimal(result[0]),
                                    nameConcept = result[1].ToString(),
                                    iva = Convert.ToDecimal(result[2])
                                });
                            }
                        }
                    }
                    if (partial.Count > 0)
                    {
                        return Ok(partial);
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo buscar el convenio") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = PartialPaymentId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para buscar el convenio" });
            }
        }
    }
}
