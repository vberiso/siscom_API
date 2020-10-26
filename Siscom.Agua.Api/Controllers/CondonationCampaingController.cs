﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.runSp;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CondonationCampaingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CondonationCampaingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCondonation()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.CondonationCampaings.Where(c => c.IsActive == true && c.StartDate < DateTime.Now && c.EndDate > DateTime.Now).ToListAsync();

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }


        [HttpPost("{idAgreement}/{idCondonation}")]
        public async Task<IActionResult> PostCondonation([FromRoute] int idAgreement, [FromRoute] int idCondonation)
        {
            string error = string.Empty;            
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[Campaing_condonation]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
                    command.Parameters.Add(new SqlParameter("@id_con_camp", idCondonation));                    
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
                        return Ok();
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo realizar la condonación de recargos por las siguientes razones: [{error}]") });
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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para cancelar recargos" });
            }

        }

        [HttpPost("CondonationPromotion/{idAgreement}/{idCondonation}")]
        public async Task<IActionResult> PostCondonationPromotion([FromRoute] int idAgreement, [FromRoute] int idCondonation)
        {
            string error = string.Empty;
            try
            {
                CondonationCampaing condonacion = await _context.CondonationCampaings.FirstOrDefaultAsync(c => c.Id == idCondonation);

                //Valido si tiene convenio
                PartialPayment partialPayment = await _context.PartialPayments.Where(p => p.AgreementId == idAgreement && p.Status == "COV01").FirstOrDefaultAsync();
                if (partialPayment != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe cancelar el convenio ya existente." });

                //Primero valido que no haya aplicado la promocion para esta cuenta
                BenefitedCampaign benefitedCampaignExiste = await _context.BenefitedCampaign.FirstOrDefaultAsync(b => b.AgreementId == idAgreement && b.DiscountCampaignId == idCondonation);
                if(benefitedCampaignExiste != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Ya se aplico esta promocion" });

                //Valido si pertenece a los tipos de predio
                Agreement contrato = await _context.Agreements.FirstOrDefaultAsync(a => a.Id == idAgreement);
                if (contrato.TypeIntakeId != 1 && contrato.TypeIntakeId != 2)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Esta promocion solo aplica para tipos: Habitacional y Comercial" });

                List<string> statuses = await _context.Statuses.Where(s => s.GroupStatusId == 4).Select(x => x.CodeName).ToListAsync();
                DateTime FechaInicial = condonacion.CondonationFrom;
                List<Debt> debts = await _context.Debts.Where(d => d.AgreementId == idAgreement && statuses.Contains(d.Status) && d.FromDate >= condonacion.CondonationFrom && d.UntilDate <= condonacion.CondonationUntil ).ToListAsync();

                foreach (var item in debts)
                {
                    List<runSp.SPParameters> parameters = new List<runSp.SPParameters> {
                    new runSp.SPParameters{Key ="id", Value = item.Id.ToString() },
                    new runSp.SPParameters{Key ="porcentage_value", Value = condonacion.Percentage.ToString() },
                    new runSp.SPParameters{Key ="discount_value", Value = "0" },
                    new runSp.SPParameters{Key ="text_discount", Value = condonacion.Alias, DbType= DbType.String, Size = 50 },
                    new runSp.SPParameters{Key ="option", Value = "1" },
                    new runSp.SPParameters{Key ="account_folio", Value = "", Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Size = 30 },
                    new runSp.SPParameters{Key ="Debt", Value = "", Direccion= ParameterDirection.Output, DbType= DbType.String, Size = 30 },
                    new runSp.SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}
                    };

                    var ss = await new RunSP(this, _context).runProcedureNT("billing_Adjusment", parameters);
                    var data = JObject.Parse(JsonConvert.SerializeObject(ss));
                    var SPParameters = JsonConvert.DeserializeObject<SPParameters>(JsonConvert.SerializeObject(data["paramsOut"][1]));
                }

                decimal TotalDeuda = debts.Sum(x => (x.Amount - x.OnAccount));
                decimal Descuento = (TotalDeuda * condonacion.Percentage) / 100;
                //Se agrega el contrato beneficiado.
                BenefitedCampaign benefitedCampaign = new BenefitedCampaign() 
                    { 
                        AgreementId = idAgreement, 
                        DiscountCampaignId = condonacion.Id,
                        NameCamping = condonacion.Name,
                        AmountDiscount = Descuento,
                        ApplicationDate = DateTime.Now                        
                    };
                _context.BenefitedCampaign.Add(benefitedCampaign);
                _context.SaveChanges();
                                
                return Ok();
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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para aplicar promocion" });
            }

        }
    }
}