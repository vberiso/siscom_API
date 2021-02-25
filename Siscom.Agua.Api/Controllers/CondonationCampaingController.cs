using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model.Promos;
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

            var discount = await _context.CondonationCampaings
                .Where(c => c.IsActive == true 
                    && c.StartDate < DateTime.Now 
                    && c.EndDate > DateTime.Now
                    && (c.Name.Contains("CDN") || c.Name.Contains("DSC"))
                    )
                .ToListAsync();

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }

        [HttpGet("byAnual/{name}")]
        public async Task<IActionResult> GetCondonation([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.CondonationCampaings.Where(c => c.Name.Equals(name)).FirstOrDefaultAsync();

            if (discount == null)
            {
                return NoContent();
            }

            return Ok(discount);
        }

        [HttpGet("SearchFor")]
        public async Task<IActionResult> GetSearchFor([FromQuery]string text)
        {
            try
            {
                var discount = await _context.CondonationCampaings
                                            .Where(c => c.Name.Contains(text) 
                                                && c.IsActive == true 
                                                && c.StartDate < DateTime.Now 
                                                && c.EndDate > DateTime.Now)
                                            .ToListAsync();
                                
                if (discount == null)
                {
                    return NoContent();
                }

                return Ok(discount);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = ex.Message });
            }
        }

        [HttpGet("Promociones")]
        public async Task<IActionResult> getPromociones()
        {
            try
            {
                var discount = await _context.CondonationCampaings
                                            .Where(c => c.IsActive == true
                                                && c.StartDate <= DateTime.Now
                                                && c.EndDate >= DateTime.Now)
                                            .ToListAsync();

                if (discount == null)
                {
                    return NoContent();
                }

                return Ok(discount);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = ex.Message });
            }
        }

        [HttpGet("cintilloName")]
        public async Task<IActionResult> getCintilloName([FromQuery] string name)
        {
            try
            {
                string ruta = @"C:\Inetpub\wwwroot\img\api_caja\" + name;
                Byte[] bytes = System.IO.File.ReadAllBytes(ruta);
                String fileBase64 = Convert.ToBase64String(bytes);

                if (string.IsNullOrEmpty(fileBase64))
                {
                    return NoContent();
                }

                return Ok(fileBase64);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = ex.Message });
            }
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
        public async Task<IActionResult> PostCondonationPromotion([FromRoute] int idAgreement, [FromRoute] int idCondonation, [FromQuery] string us = "", [FromQuery] string usName = "")
        {
            string error = string.Empty;
            try
            {
                CondonationCampaing condonacion = await _context.CondonationCampaings.FirstOrDefaultAsync(c => c.Id == idCondonation);
                PromotionCaja promotion = JsonConvert.DeserializeObject<PromotionCaja>(condonacion.Alias);
                promotion.Id = condonacion.Id;

                //Valido si tiene convenio
                PartialPayment partialPayment = await _context.PartialPayments.Where(p => p.AgreementId == idAgreement && p.Status == "COV01").FirstOrDefaultAsync();
                if (partialPayment != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe cancelar el convenio ya existente." });

                //Primero valido que no haya aplicado la promocion para esta cuenta
                BenefitedCampaign benefitedCampaignExiste = await _context.BenefitedCampaign.FirstOrDefaultAsync(b => b.AgreementId == idAgreement && b.DiscountCampaignId == idCondonation);
                if (benefitedCampaignExiste != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Ya se aplico esta promocion" });

                //Valido si pertenece a los tipos de predio
                if (promotion.TiposToma.Count > 0)
                {
                    Agreement contrato = await _context.Agreements.FirstOrDefaultAsync(a => a.Id == idAgreement);
                    if (!promotion.TiposToma.Contains(contrato.TypeIntakeId))
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Esta promoción no aplica para el tipo de toma del contrato." });
                }


                List<string> statuses = await _context.Statuses.Where(s => s.GroupStatusId == 4).Select(x => x.CodeName).ToListAsync();
                List<Debt> debts = new List<Debt>();                
                foreach (var item in promotion.Condonaciones)
                {
                    List<Debt> debtsTmp = await _context.Debts.Include(x => x.DebtDetails).Where(d =>
                       d.AgreementId == idAgreement
                       && statuses.Contains(d.Status)
                       && item.tipos.Contains(d.Type)
                       && d.FromDate >= item.condonacionDeudaDesde
                       && d.UntilDate <= item.condonacionDeudaHasta
                        ).ToListAsync();

                    List<DebtStatus> debtStatuses = new List<DebtStatus>();
                    foreach (var deuda in debtsTmp)
                    {
                        if (!deuda.Type.Equals("TIP02"))
                        {
                            deuda.Status = "ED009";

                            DebtStatus debtStatus = new DebtStatus();
                            debtStatus.id_status = "ED009";
                            debtStatus.DebtStatusDate = DateTime.Now;
                            debtStatus.User = promotion.Nombre;
                            debtStatus.DebtId = deuda.Id;
                            debtStatuses.Add(debtStatus);
                        }
                        else if (deuda.Type.Equals("TIP02") 
                            && deuda.DebtDetails.Count == 1 
                            && item.codes.Contains(int.Parse(deuda.DebtDetails.FirstOrDefault().CodeConcept)))
                        {
                            deuda.Status = "ED009";

                            DebtStatus debtStatus = new DebtStatus();
                            debtStatus.id_status = "ED009";
                            debtStatus.DebtStatusDate = DateTime.Now;
                            debtStatus.User = promotion.Nombre;
                            debtStatus.DebtId = deuda.Id;
                            debtStatuses.Add(debtStatus);
                        }
                    }

                    await _context.DebtStatuses.AddRangeAsync(debtStatuses);
                    await _context.SaveChangesAsync();

                    debts.AddRange(debtsTmp);
                }
                 
                AgreementComment agreementComment = new AgreementComment()
                {
                    AgreementId = idAgreement,
                    DateIn = DateTime.Now,
                    Observation = promotion.Nombre,
                    IsVisible = true,
                    UserId = us,
                    UserName = usName,
                    DateOut = DateTime.Now
                };
                _context.AgreementComments.Add(agreementComment);
                await _context.SaveChangesAsync();

                decimal TotalDeuda = debts.Sum(x => (x.Amount - x.OnAccount));

                //Se agrega el contrato beneficiado.
                if (promotion.Nombre.Substring(0, 3).Equals("CDN"))
                {
                    BenefitedCampaign benefitedCampaign = new BenefitedCampaign()
                    {
                        AgreementId = idAgreement,
                        DiscountCampaignId = promotion.Id,
                        NameCamping = promotion.Nombre,
                        AmountDiscount = TotalDeuda,
                        ApplicationDate = DateTime.Now
                    };
                    _context.BenefitedCampaign.Add(benefitedCampaign);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { condonado = TotalDeuda });
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

        [HttpPost("DiscountPromotion/{idAgreement}/{idCondonation}")]
        public async Task<IActionResult> PostDiscountPromotion([FromRoute] int idAgreement, [FromRoute] int idCondonation)
        {
            string error = string.Empty;
            try
            {
                CondonationCampaing condonacion = await _context.CondonationCampaings.FirstOrDefaultAsync(c => c.Id == idCondonation);
                PromotionCaja promotion = JsonConvert.DeserializeObject<PromotionCaja>(condonacion.Alias);
                promotion.Id = condonacion.Id;

                //Valido si tiene convenio
                PartialPayment partialPayment = await _context.PartialPayments.Where(p => p.AgreementId == idAgreement && p.Status == "COV01").FirstOrDefaultAsync();
                if (partialPayment != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe cancelar el convenio ya existente." });

                //Primero valido que no haya aplicado la promocion para esta cuenta
                BenefitedCampaign benefitedCampaignExiste = await _context.BenefitedCampaign.FirstOrDefaultAsync(b => b.AgreementId == idAgreement && b.DiscountCampaignId == idCondonation);
                if (benefitedCampaignExiste != null)
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Ya se aplico esta promocion" });

                //Valido si pertenece a los tipos de predio
                if (promotion.TiposToma.Count > 0)
                {
                    Agreement contrato = await _context.Agreements.FirstOrDefaultAsync(a => a.Id == idAgreement);
                    if (!promotion.TiposToma.Contains(contrato.TypeIntakeId))
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Esta promoción no aplica para el tipo de toma del contrato." });
                }                

                List<string> statuses = await _context.Statuses.Where(s => s.GroupStatusId == 4).Select(x => x.CodeName).ToListAsync();
                                
                foreach (var descuento in promotion.Descuentos)
                {
                    List<Debt> debtsTmp = await _context.Debts.Include(x => x.DebtDetails).Where(d =>
                       d.AgreementId == idAgreement
                       && statuses.Contains(d.Status)
                       && descuento.tipos.Contains(d.Type)
                       && d.FromDate >= descuento.condonacionDeudaDesde
                       && d.UntilDate <= descuento.condonacionDeudaHasta
                        ).ToListAsync();

                    foreach (var item in debtsTmp)
                    {
                        if (!item.Type.Equals("TIP02"))
                        {
                            List<runSp.SPParameters> parameters = new List<runSp.SPParameters> {
                                new runSp.SPParameters{Key ="id", Value = item.Id.ToString() },
                                new runSp.SPParameters{Key ="porcentage_value", Value = descuento.descuento.ToString() },
                                new runSp.SPParameters{Key ="discount_value", Value = "0" },
                                new runSp.SPParameters{Key ="text_discount", Value = promotion.ObservacionFactura, DbType= DbType.String, Size = 50 },
                                new runSp.SPParameters{Key ="option", Value = "1" },
                                new runSp.SPParameters{Key ="account_folio", Value = "", Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Size = 30 },
                                new runSp.SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}
                            };

                            var ss = await new RunSP(this, _context).runProcedureNT("dbo.billing_Adjusment", parameters);
                            var data = JObject.Parse(JsonConvert.SerializeObject(ss));
                            var SPParameters = JsonConvert.DeserializeObject<SPParameters>(JsonConvert.SerializeObject(data["paramsOut"][1]));
                        }
                        else if (item.Type.Equals("TIP02")
                            && item.DebtDetails.Count == 1
                            && descuento.codes.Contains(int.Parse(item.DebtDetails.FirstOrDefault().CodeConcept)))
                        {
                            List<runSp.SPParameters> parameters = new List<runSp.SPParameters> {
                                new runSp.SPParameters{Key ="id", Value = item.Id.ToString() },
                                new runSp.SPParameters{Key ="porcentage_value", Value = descuento.descuento.ToString() },
                                new runSp.SPParameters{Key ="discount_value", Value = "0" },
                                new runSp.SPParameters{Key ="text_discount", Value = promotion.ObservacionFactura, DbType= DbType.String, Size = 50 },
                                new runSp.SPParameters{Key ="option", Value = "1" },
                                new runSp.SPParameters{Key ="account_folio", Value = "", Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Size = 30 },
                                new runSp.SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}
                            };

                            var ss = await new RunSP(this, _context).runProcedureNT("dbo.billing_Adjusment", parameters);
                            var data = JObject.Parse(JsonConvert.SerializeObject(ss));
                            var SPParameters = JsonConvert.DeserializeObject<SPParameters>(JsonConvert.SerializeObject(data["paramsOut"][1]));
                        }
                    }
                }

                //Se obtiene la suma del descuento.
                List<Debt> debts = new List<Debt>();
                foreach (var descuento in promotion.Descuentos)
                {
                    List<Debt> debtsTmp = await _context.Debts.Where(d =>
                       d.AgreementId == idAgreement
                       && statuses.Contains(d.Status)
                       && descuento.tipos.Contains(d.Type)
                       && d.FromDate >= descuento.condonacionDeudaDesde
                       && d.UntilDate <= descuento.condonacionDeudaHasta
                        ).ToListAsync();
                    debts.AddRange(debtsTmp);                    
                }
                List<int> idsDebt = debts.Select(d => d.Id).ToList();
                List<DebtDiscount> debtDiscounts = await _context.DebtDiscounts.Where(dd => idsDebt.Contains(dd.DebtId)).ToListAsync();
                decimal Descuento = debtDiscounts.Sum(dd => dd.DiscountAmount);

                //Se agrega el contrato beneficiado.
                if (promotion.Nombre.Substring(0, 3).Equals("DSC"))
                {
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
                }
                
                return Ok(new { descuento = Descuento });
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

        [HttpPost("RegisterBenefitedAccount/{idAgreement}/{idCondonation}/{total}")]
        public async Task<IActionResult> RegisterBenefitedAccount([FromRoute] int idAgreement, [FromRoute] int idCondonation, [FromRoute] decimal total)
        {
            try
            {
                CondonationCampaing condonacion = await _context.CondonationCampaings.FirstOrDefaultAsync(c => c.Id == idCondonation);
                PromotionCaja promotion = JsonConvert.DeserializeObject<PromotionCaja>(condonacion.Alias);
                promotion.Id = condonacion.Id;

                BenefitedCampaign benefitedCampaign = new BenefitedCampaign()
                {
                    AgreementId = idAgreement,
                    DiscountCampaignId = condonacion.Id,
                    NameCamping = condonacion.Name,
                    AmountDiscount = total,
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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para registrar la cuenta beneficiada de promocion" });
            }
        }
            

        [HttpPost("BorrarDeudaDeAño/{idAgreement}/{año}/{usuario}" )]
        public async Task<IActionResult> PostBorrarDeudaDeAño([FromRoute] int idAgreement, [FromRoute] int año, [FromRoute] string usuario)
        {
            string error = string.Empty;
            try
            {
                var statuses = await _context.Statuses.Where(s => s.GroupStatusId == 4).ToListAsync();
                var deuda = await _context.Debts.Where(
                                                    d => d.AgreementId == idAgreement
                                                    && statuses.Select(x => x.CodeName).Contains(d.Status)
                                                    && d.Year == año)
                                                .ToListAsync();

                //Se cambia el status a cancelado
                List<DebtStatus> debtStatuses = new List<DebtStatus>();
                foreach (var item in deuda)
                {
                    item.Status = "ED006";

                    DebtStatus debtStatus = new DebtStatus()
                    {
                        id_status = "ED006",
                        DebtStatusDate = DateTime.Now,
                        User = usuario,
                        DebtId = item.Id
                    };
                    debtStatuses.Add(debtStatus);
                }
                _context.Debts.UpdateRange(deuda);
                await _context.SaveChangesAsync();

                //Se genera el historial
                _context.DebtStatuses.AddRange(debtStatuses);
                await _context.SaveChangesAsync();

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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para borrar deuda" });
            }

        }

    }
}