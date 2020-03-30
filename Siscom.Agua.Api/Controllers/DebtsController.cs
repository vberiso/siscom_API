using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class DebtsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DebtsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("id/{idDebt}")]
        public async Task<IActionResult> getDebtId([FromRoute] int idDebt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var debt = await _context.Debts
                        .Include(d => d.DebtDetails)
                        .Include(d => d.DebtDiscounts)
                        .Include(d => d.DebtStatuses)
                        .Where(d => d.Id == idDebt)
                        .FirstOrDefaultAsync() ;

            if (debt == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se ha encontrado deuda activa para esta cuenta" });
            }

            return Ok(debt);
        }

        // GET: api/Debts/5
        [HttpGet("{idAgreement}")]
        public async Task<IActionResult> GetDebt([FromRoute] int idAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var debt = await _context.Debts.Include(dd => dd.DebtDetails)
                        .Where(gs => _context.Statuses
                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == idAgreement).OrderBy(x => x.FromDate).ToListAsync();
            var status = await _context.Statuses.ToListAsync();
            var type = await _context.Types.ToListAsync();

            debt.ForEach(x =>
            {
                x.DescriptionStatus = (from d in status
                                       where d.CodeName == x.Status
                                       select d).FirstOrDefault().Description;

                x.DescriptionType = (from d in type
                                     where d.CodeName == x.Type
                                     select d).FirstOrDefault().Description;

            });


            if (debt == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se ha encontrado deuda activa para esta cuenta" });
            }

            return Ok(debt);
        }

        // GET: api/Debts/5
        [HttpGet("History/{idAgreement}")]
        public async Task<IActionResult> GetHistoryDebt([FromRoute] int idAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var debt = await _context.Debts
                                    .Include(x => x.DebtDetails)
                                    .Where(gs => gs.AgreementId == idAgreement).OrderByDescending(x => x.FromDate).ToListAsync();
            var status = await _context.Statuses.ToListAsync();
            var type = await _context.Types.ToListAsync();

            debt.ForEach(x =>
            {
                x.DescriptionStatus = (from d in status
                                       where d.CodeName == x.Status
                                       select d).FirstOrDefault().Description;

                x.DescriptionType = (from d in type
                                     where d.CodeName == x.Type
                                     select d).FirstOrDefault().Description;

            });


            if (debt == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se ha encontrado deuda activa para esta cuenta" });
            }

            return Ok(debt);
        }

        // GET: api/Debts/GetDebtByPeriod/5
        [HttpGet("GetDebtByPeriod/{idAgreement}")]
        public async Task<IActionResult> GetDebtByPeriod([FromRoute] int idAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var debt = await _context.Debts
                                     .Include(a => a.Agreement)
                                        .ThenInclude(ad => ad.AgreementDetails)
                                     .Include(dd => dd.DebtDetails)
                                        

                                     .Where(gs => gs.AgreementId == idAgreement)
                                     .OrderByDescending(x => x.DebitDate.Year).ToListAsync();

            var status = await _context.Statuses.ToListAsync();
            var type = await _context.Types.ToListAsync();

            debt.ForEach(x =>
            {
                x.DescriptionStatus = (from d in status
                                       where d.CodeName == x.Status
                                       select d).FirstOrDefault().Description;

                x.DescriptionType = (from d in type
                                     where d.CodeName == x.Type
                                     select d).FirstOrDefault().Description;
            });

            if (debt.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                                  new { Error = "No tiene recibos" });
            }

            return Ok(debt);
        }
       
        [HttpGet("Detail/{DebtId}")]
        public async Task<IActionResult> GetDebtDetail([FromRoute] int DebtId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var DebtDiscount = await _context.DebtDiscounts
                                        .Where(x => x.DebtId == DebtId)
                                        .ToListAsync();

            var Detail = await _context.DebtDetails
                                     .Where(d => d.DebtId== DebtId)
                                     .OrderBy(x => x.NameConcept).ToListAsync();

            if (Detail == null)
            {
                return NotFound();
            }

            return Ok(new { Detail, DebtDiscount });
        }

        [HttpPost("GetDiscountAnnual/{porcentajeDiscount}")]
        public async Task<IActionResult> CheckIsAnnual([FromRoute] int porcentajeDiscount, [FromBody] List<int> DebtIds)
        {
            decimal descuento = 0;
            var AllDebtAnnual = new List<int>();
            bool ApplyMSI = false;
            List<int> idsDeb = new List<int>();
            try
            {
                DebtIds.ForEach(DebtId =>
                {
                    var result = _context.PagosAnuales.Where(x => x.DebtId == DebtId).FirstOrDefault();
                    var result2 = _context.PromotionDebt.Where(x => x.DebtId == DebtId).FirstOrDefault();
                    if (result != null)
                    {

                        if (result2 != null)
                        {
                            ApplyMSI = true;
                            idsDeb.Add(DebtId);
                        }
                        else
                        {
                            var debt = _context.Debts.Where(x => x.Id == DebtId).FirstOrDefault();
                            

                             descuento = descuento + Math.Round((((debt.Amount - debt.OnAccount) * porcentajeDiscount) / 100), 2);
                        }
                        AllDebtAnnual.Add(DebtId);
                    }
                });
                return Ok(new { descuento , ApplyMSI, ids= idsDeb, AllDebtAnnual });
            }
            catch (Exception e)
            {
                return Conflict(new { error = "Error" });
            }
        }

        [HttpPost("Condonation/{user}/{comentario}")]
        public async Task<IActionResult> Condonation([FromRoute] string user, [FromRoute] string comentario,[FromBody] List<int> Ids)
        {
            try
            {
                var debts = await _context.Debts.Where(d => Ids.Contains(d.Id)).ToListAsync();
                var Usuario = _context.Users.FirstOrDefault(u => u.Id == user);
                
                AgreementComment ac = new AgreementComment();
                ac.DateIn = DateTime.Now;
                ac.Observation = comentario;
                ac.IsVisible = true;
                ac.UserId = user;
                ac.UserName = Usuario.Name + " " + Usuario.LastName + " " + Usuario.SecondLastName;
                ac.AgreementId = (int)debts.FirstOrDefault()?.AgreementId;
                _context.AgreementComments.Add(ac);

                foreach (var item in debts)
                {                    
                    item.Status = "ED006";
                    _context.Debts.Update(item);

                    DAL.Models.DebtStatus debtStatus = new DAL.Models.DebtStatus();
                    debtStatus.id_status = "ED006";
                    debtStatus.DebtStatusDate = DateTime.Now;
                    debtStatus.User = Usuario.Name + " " + Usuario.LastName + " " + Usuario.SecondLastName;
                    debtStatus.DebtId = item.Id;
                    _context.DebtStatuses.Add(debtStatus);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = ex.Message;
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "DebtController";
                systemLog.Action = "PostCondonation";
                systemLog.Parameter = JsonConvert.SerializeObject(Ids);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para condonar debts." });
            }            
        }

    }
}