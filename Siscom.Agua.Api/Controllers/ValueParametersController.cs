using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    public class ValueParametersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ValueParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllParameters")]
        public async Task<IEnumerable<SystemParameters>> GetAll()
        {
            var today = DateTime.UtcNow.ToLocalTime().Date;
            return (await _context.SystemParameters.Where(x => x.IsActive == true && (x.StartDate <= today && x.EndDate >= today)) .ToListAsync());
        }

        [HttpGet("GetParametersById/{id}")]
        public async Task<IActionResult> GetParametersById([FromRoute] int id)
        {
            if (!ParameterExists(id))
            {
                return BadRequest();
            }

            return Ok(await _context.SystemParameters.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SystemParameters system)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _context.SystemParameters.AddAsync(system);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(system);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el parametro" });
            }
            return Ok(system);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] SystemParameters system)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != system.Id)
            {
                return BadRequest();
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var param = await _context.SystemParameters.FindAsync(id);
                    param.IsActive = system.IsActive;
                    param.Name = system.Name;
                    param.NumberColumn = system.NumberColumn;
                    param.StartDate = system.StartDate;
                    param.TextColumn = system.TextColumn;
                    param.TypeColumn = system.TypeColumn;
                    param.DateColumn = system.DateColumn;
                    param.EndDate = system.EndDate;

                    _context.Entry(param).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(system);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el parametro" });
            }

            return Ok(system);
        }

        [HttpGet]
        public IActionResult Get(string value)
        {
            var today = DateTime.UtcNow.ToLocalTime().Date;
            var b = value.ToUpper();
            SystemParameters val;
            if(value == "ANUAL")
                val = _context.SystemParameters.Where(x => x.Name == b && x.IsActive == true && (x.StartDate.Year <= today.Year && x.EndDate >= today)).FirstOrDefault();
            else
                val = _context.SystemParameters.Where(x => x.Name == b && x.IsActive == true && (x.StartDate <= today && x.EndDate >= today)).FirstOrDefault();
            return Ok(val);
        }

        [HttpGet("ReconnectionCosts")]
        public async Task<IActionResult> GetReconnectionCosts()
        {
            List<ReconocetionCostsVM> Product = new List<ReconocetionCostsVM>();

            var a = await _context.SystemParameters
                .Where(x => x.Name.Contains("RECO") && x.IsActive)
                .Select(x => new { x.TextColumn, x.Name })
                .ToListAsync();

            foreach (var item in a)
            {
                item.TextColumn.Split(",")
                    .Select(p => Int32.TryParse(p, out int n) ? n : (int?)null)
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToList()
                    .ForEach(x =>
                    {

                        TariffProduct tariff = _context.TariffProducts
                        .Include(z => z.Product)
                        .Where(z => z.ProductId == x && z.IsActive == 1)
                        .FirstOrDefault();

                        ReconocetionCostsVM vM = new ReconocetionCostsVM();
                        vM.IdReconection = tariff.ProductId;
                        vM.Amount = tariff.Amount;
                        vM.HaveTax = tariff.HaveTax;
                        vM.ProductName = tariff.Product.Name;
                        vM.Type = tariff.Type;
                        switch (item.Name) {
                            case "RECO1":
                                vM.TypeIntake = "HABITACIONAL";
                            break;
                            case "RECO2":
                                vM.TypeIntake = "COMERCIAL";
                                break;
                            case "RECO3":
                                vM.TypeIntake = "INDUSTRIAL";
                                break;
                        };
                        Product.Add(vM);
                    });
            }
            return Ok(Product);
        }

        [HttpGet("ReasonCatalog")]
        public async Task<IActionResult> GetReasonCatalog()
        {
            return Ok( await _context.ReasonCatalog.Select(x => new
            {
                Id = x.Id,
                Description = x.Description,
                IsActive = x.IsActive,
                Type = x.Type
            }).ToListAsync());
        }

        [HttpGet("ReasonOrders")]
        public async Task<IActionResult> GetReasonOrders()
        {
            return Ok(await _context.GroupLists.Include(x => x.Lists).ToListAsync());
        }

        [HttpGet("GetAgreementCatalog")]
        public async Task<IActionResult> GetAgreementCatalog()
        {
            TypesAgreementVM types = new TypesAgreementVM();
            types.TypeClassifications = await _context.TypeClassifications
                .Where(x => x.IsActive)
                .Select(x => new TypeClassificationsVM
                {
                    Id = x.Id,
                    IntakeAcronym = x.IntakeAcronym,
                    Name = x.Name
                }).ToListAsync();

            types.TypeIntake = await _context.TypeIntakes
                .Where(x => x.IsActive)
                .Select(x => new TypeIntakeVM
                {
                    Id = x.Id,
                    Acronym = x.Acronym,
                    Name = x.Name
                }).ToListAsync();

            types.TypeService = await _context.TypeServices
               .Where(x => x.IsActive)
               .Select(x => new TypeServiceVM
               {
                   Id = x.Id,
                   Name = x.Name
               }).ToListAsync();

            types.TypeUse = await _context.TypeUses
               .Where(x => x.IsActive)
               .Select(x => new TypeUseVM
               {
                   Id = x.Id,
                   Name = x.Name,
                   IntakeAcronym = x.IntakeAcronym
               }).ToListAsync();

            return Ok(types);
        }

        [HttpGet("OrderWorkList/{id}")]
        public async Task<IActionResult> GetOrderWorkList([FromRoute] int id)
        {
            return Ok(await _context.OrderWorkLists.Where(x => x.OrderWorkId == id).ToListAsync());
        }

        [HttpGet("Campaign")]
        public async Task<IActionResult> ExistDiscount()
        {
            var campaign = await _context.DiscountCampaigns.Where(x => x.IsActive &&
                                                            x.StartDate <= DateTime.Now.Date &&
                                                            x.EndDate >= DateTime.Now.Date &&
                                                            !x.Name.Contains("INFDES"))
                                                            .ToListAsync();
            return Ok(campaign);
        }

        [HttpGet("Condonations")]
        public async Task<IActionResult> ExistCondonations()
        {
            var tmpCond = await _context.CondonationCampaings.Where(c => c.IsActive == true &&
                                                                            c.StartDate < DateTime.Now &&
                                                                            c.EndDate > DateTime.Now).ToListAsync();
            return Ok(tmpCond);
        }

        [HttpGet("{mac}")]
        public async Task<IActionResult> FindTerminalMac([FromRoute] string mac)
        {
            string valores = String.Empty;
            Terminal terminal = new Terminal();
            TerminalUser terminalUser = new TerminalUser();

            if (!String.IsNullOrEmpty(mac))
            {
                terminal = await _context.Terminal
                                         .Include(x => x.BranchOffice)
                                         .Include(x => x.TerminalUsers)
                                         .Where(x => x.MacAdress == mac &&
                                                     x.IsActive == true).FirstOrDefaultAsync();
                if(terminal != null)
                    terminal.TerminalUsers = terminal.TerminalUsers.Where(x => x.InOperation == true).ToList();
            }

            if (terminal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(terminal);
            }
        }

        [HttpGet("Terminal/{teminalUserId}")]
        public async Task<IActionResult> GetTransactionCashBox([FromRoute] int teminalUserId)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            int _typeTransaction = 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Where(x => x.Id == teminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
            {
                return NotFound();
            }


            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                            x.TerminalUser.InOperation == true )
                                                .OrderBy(x => x.Id).ToListAsync();

            foreach (var item in movimientosCaja)
            {
                switch (item.TypeTransaction.Id)
                {
                    case 1://apertura
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 2://Fondo
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 5://Cierre
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 7: //Liquidada
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                }
            }

            return Ok(_typeTransaction);
        }

        private bool ParameterExists(int id)
        {
            return _context.SystemParameters.Any(e => e.Id == id);
        }

        [HttpGet("GetConfiguration")]
        public async Task<IActionResult> GetConfiguration([FromRoute] int teminalUserId)
        {
            

           

            return Ok("Ok");
        }
                
    }
}