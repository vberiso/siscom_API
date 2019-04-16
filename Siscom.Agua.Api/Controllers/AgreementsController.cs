using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class AgreementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;

        public AgreementsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        // GET: api/Agreements
        [HttpGet("GetSummary/{Account}")]
        public async Task<IActionResult> GetAgreements([FromRoute] string Account)
        {
            var summary = await _context.Agreements
                                    .Include(a => a.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                            .ThenInclude(t => t.Towns)
                                                .ThenInclude(st => st.States)
                                    .Include(c => c.Clients)
                                    .Include(ti => ti.TypeIntake)
                                    .Include(ts => ts.TypeStateService)
                                    .Include(di => di.AgreementDiscounts)
                                        .ThenInclude(d => d.Discount)
                                    .Include(p => p.Prepaids)
                                    .Include(a=> a.AgreementLogs)
                                    .Where(a => a.Account == Account).FirstOrDefaultAsync();

            summary.Clients = summary.Clients.Where(c => c.IsActive == true).ToList();
            summary.Addresses = summary.Addresses.Where(a => a.TypeAddress == "DIR01" && a.IsActive == true).ToList();

            var status = await _context.Statuses.ToListAsync();
            var type = await _context.Types.ToListAsync();

            summary.Prepaids.ToList().ForEach(x =>
            {  
                if(!string.IsNullOrEmpty(x.Status))
                x.StatusDescription = (from d in status
                                       where d.CodeName == x.Status
                                       select d).FirstOrDefault().Description;

                if (!string.IsNullOrEmpty(x.Type))
                    x.TypeDescription = (from d in type
                                     where d.CodeName == x.Type
                                     select d).FirstOrDefault().Description;
            });


            summary.Debts = await _context.Debts
                                          .Include( gs => gs.DebtDetails)
                                          .Where(gs => _context.Statuses
                                          .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == summary.Id).ToListAsync();    

            return Ok(summary);
        }


        // GET: api/Agreements/5
        [HttpGet("{id}", Name = "GetAgreementById" )]
        public async Task<IActionResult> GetAgreement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var currentUserName = this.User.Claims.ToList()[1].Value;
            var agreement = await GetAgreementData(id);


            if (agreement == null)
            {
                return NotFound();
            }

            return Ok(agreement);
        }



        [HttpGet("AgreementByAccount/{AcountNumber}")]
        public async Task<IActionResult> GetGetAgreementByAccount([FromRoute] string AcountNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agreement = await _context.Agreements
                                      .Include(x => x.Clients)
                                        .ThenInclude(contact => contact.Contacts)
                                      .Include(x => x.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                      .Include(ts => ts.TypeService)
                                      .Include(tu => tu.TypeUse)
                                      .Include(tc => tc.TypeConsume)
                                      .Include(tr => tr.TypeRegime)
                                      .Include(tp => tp.TypePeriod)
                                      .Include(tcb => tcb.TypeCommertialBusiness)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ti => ti.TypeIntake)
                                      .Include(di => di.Diameter)
                                      .Include(tc => tc.TypeClassification)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .Include(ad => ad.AgreementDiscounts)
                                      .ThenInclude(d => d.Discount)
                                      .Include(ad => ad.AgreementDetails)
                                      .Include(af => af.AgreementFiles)
                                      .FirstOrDefaultAsync(a => a.Account == AcountNumber);

            agreement.Addresses.ToList().ForEach(x =>
            {
                x.Suburbs = _context.Suburbs.Include(r => r.Regions)
                                            .Include(c => c.Clasifications)
                                            .Include(t => t.Towns)
                                                .ThenInclude(s => s.States)
                                                .ThenInclude(c => c.Countries)
                                            .Where(i => i.Id == x.Suburbs.Id)
                                            .SingleOrDefault();
            });


            if (agreement == null)
            {
                return NotFound();
            }

            return Ok(agreement);
        }

        // GET: api/Agreements/
        [HttpGet("AgreementByAccount/Cash/{AcountNumber}")]
        public async Task<IActionResult> GetGetAgreementByAccountCash([FromRoute] string AcountNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var agreement = await _context.Agreements
                                     .Include(x => x.Clients)
                                     .Include(x => x.Addresses)
                                       .ThenInclude(s => s.Suburbs)
                                           .ThenInclude(t => t.Towns)
                                               .ThenInclude(st => st.States)
                                     .FirstOrDefaultAsync(a => a.Account == AcountNumber);
                if (agreement == null)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "No hay datos para este número de cuenta" });
                }

                agreement.Clients = agreement.Clients.Where(c => c.TypeUser == "CLI01" && c.IsActive == true).ToList();
                agreement.Addresses = agreement.Addresses.Where(c => c.TypeAddress == "DIR01" && c.IsActive == true).ToList();

                return Ok(agreement);

            }
            catch (Exception e)
            {

            }
            return NotFound();
         
        }
        // GET: api/Agreements
        [HttpGet("AgreementsBasic/{id}")]
        public async Task<IActionResult> GetAgreementsBasic([FromRoute] int id)
        {
            var agreement = await _context.Agreements
                                      .Include(ts => ts.TypeService)
                                      .Include(tu => tu.TypeUse)
                                      .Include(tc => tc.TypeConsume)
                                      .Include(tr => tr.TypeRegime)
                                      .Include(tp => tp.TypePeriod)
                                      .Include(tcb => tcb.TypeCommertialBusiness)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ti => ti.TypeIntake)
                                      .Include(di => di.Diameter)
                                      .Include(c => c.Clients)
                                      .Include(tc => tc.TypeClassification)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .Include(ad => ad.AgreementDetails)
                                      .Include(af => af.AgreementFiles)
                                      .FirstOrDefaultAsync(a => a.Id == id);
            var service = agreement.AgreementServices.Where(x => x.IsActive == false);
            agreement.AgreementServices = agreement.AgreementServices.Except(service).ToList();

            if (agreement == null)
            {
                return NotFound();
            }
            return Ok(agreement);
        }

        [HttpGet("GetDerivatives/{id}")]
        public async Task<IActionResult> GetDerivatives([FromRoute] int id)
        {






            //change

            //with fix

            List<DerivativesVM> dero = new List<DerivativesVM>();
            var deriv = await _context.Derivatives
                                            .Include(x=>x.Agreement)
                                            .Where(a => a.AgreementId == id).ToListAsync();

            foreach (var client in deriv)
            {
                var accountDerivatives = _context.Agreements.Where(a => a.Id == client.AgreementDerivative).FirstOrDefault();
                dero.Add(new DerivativesVM()
                {
                    Id = client.Id,
                    AccountAgreement = client.Agreement.Account,
                    AccountDerivative = accountDerivatives.Account,
                    IsActive = client.IsActive,
                    IdDerivative = accountDerivatives.Id

                });

            }
            if (dero.Count == 0)
            {
                return NotFound();
            }
            return Ok(dero);
        }

        [HttpGet("GetDerivativesDos/{id}")]
        public async Task<IActionResult> GetDerivativesDos([FromRoute] int id)
        {
            List<DerivativesVM> dero = new List<DerivativesVM>();
            var deriv = await _context.Derivatives
                                        .Include(x => x.Agreement)
                                        .Where(a => a.AgreementDerivative == id).ToListAsync();



            foreach (var client in deriv)
            {
                var accountDerivatives = _context.Agreements.Where(a => a.Id == client.AgreementDerivative).FirstOrDefault();
                dero.Add(new DerivativesVM()
                {
                    Id = client.Id,
                    AccountAgreement = client.Agreement.Account,
                    AccountDerivative = accountDerivatives.Account,
                    IsActive = client.IsActive,
                    IdDerivative = accountDerivatives.Id
                });

            }

            if (dero.Count == 0)
            {
                return NotFound();
            }
            return Ok(dero);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        ///  /// <remarks>
        /// Types request:
        ///
        /// Type: {
        ///     1 => Search by Account
        ///     2 => Search by Name of Client
        ///     3 => Search by Address of Client
        ///     4 => Search by RFC of Client
        /// }
        /// StringSearch:{
        ///     String for type
        /// }
        ///
        /// </remarks>
        [HttpGet("FindAgreementParam")]
        public async Task<IActionResult> FindAgreementParam([FromQuery] SearchAgreementVM search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            search.StringSearch.Replace("%20", " ");
            var dataTable = new System.Data.DataTable();
            List<FindAgreementParamVM> clientsFilter = new List<FindAgreementParamVM>();
            switch (search.Type)
            {
                case 1:
                    try
                    {
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT A.id_agreement " +
                                    ",A.account Account " +
                                    ",CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name) Nombre " +
                                    ",C.rfc RFC " +
                                    ",TSS.id_type_state_service idStus " +
                                    ",TSS.name [Status] " +
                                    ",COUNT(ADI.id_discount) " +
                                    ",CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name) " +
                                    ",A.type_agreement Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE A.account = '" + search.StringSearch + "' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), A.type_agreement, A.num_derivatives";
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    //dataTable.Load(result);
                                    //foreach (System.Data.DataRow item in dataTable.Rows)
                                    //{
                                    //    clientsFilter.Add(new FindAgreementParamVM
                                    //    {
                                    //        AgreementId = Convert.ToInt32(item[0]),
                                    //        Account = item[1].ToString(),
                                    //        Nombre = item[2].ToString().ToUpper(),
                                    //        RFC = item[3].ToString(),
                                    //        idStus = Convert.ToInt32(item[4]),
                                    //        Status = item[5].ToString(),
                                    //        WithDiscount = Convert.ToBoolean(item[6]),
                                    //        Address = item[7].ToString()
                                    //    });
                                    //}
                                    while (await result.ReadAsync())
                                    {
                                        clientsFilter.Add(new FindAgreementParamVM
                                        {
                                            AgreementId = Convert.ToInt32(result[0]),
                                            Account = result[1].ToString(),
                                            Nombre = result[2].ToString().ToUpper(),
                                            RFC = result[3].ToString(),
                                            idStus = Convert.ToInt32(result[4]),
                                            Status = result[5].ToString(),
                                            WithDiscount = Convert.ToBoolean(result[6]),
                                            Address = result[7].ToString(),
                                            Type = result[8].ToString(),
                                            NumDerivades = Convert.ToInt32(result[9])
                                        });
                                    }
                                }
                            }
                        }
                        if (clientsFilter.Count > 0)
                            return Ok(clientsFilter);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    break;
                case 2:
                    search.StringSearch = search.StringSearch.ToUpper();
                    if(search.StringSearch.Length < 5)
                    {
                        return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No se pudo completar su busqueda, favor de ingresar un minimo de 5 caracteres para poder continuar" });
                    }
                    try
                    {
                       
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT A.id_agreement " +
                                    ",A.account Account " +
                                    ",CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name) Nombre " +
                                    ",C.rfc RFC " +
                                    ",TSS.id_type_state_service idStus " +
                                    ",TSS.name [Status] " +
                                    ",COUNT(ADI.id_discount) " +
                                    ",CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name) " +
                                    ",A.type_agreement Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE CONCAT(UPPER(C.name) , ' ' , UPPER(C.last_name), ' ' , UPPER(C.second_last_name)) LIKE '%" + search.StringSearch + "%' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), A.type_agreement, A.num_derivatives";
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    //dataTable.Load(result);
                                    while (await result.ReadAsync())
                                    {
                                        clientsFilter.Add(new FindAgreementParamVM
                                        {
                                            AgreementId = Convert.ToInt32(result[0]),
                                            Account = result[1].ToString(),
                                            Nombre = result[2].ToString().ToUpper(),
                                            RFC = result[3].ToString(),
                                            idStus = Convert.ToInt32(result[4]),
                                            Status = result[5].ToString(),
                                            WithDiscount = Convert.ToBoolean(result[6]),
                                            Address = result[7].ToString(),
                                            Type = result[8].ToString(),
                                            NumDerivades = Convert.ToInt32(result[9])
                                        });
                                    }
                                }
                            }
                        }
                        if (clientsFilter.Count > 0)
                            return Ok(clientsFilter);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    break;
                case 3:
                    search.StringSearch = search.StringSearch.ToUpper();
                   
                    if (search.StringSearch.Length < 5)
                    {
                        return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No se pudo completar su busqueda, favor de ingresar un minimo de 5 caracteres para poder continuar" });
                    }
                    try
                    {
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT A.id_agreement " +
                                    ",A.account Account " +
                                    ",CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name) Nombre " +
                                    ",C.rfc RFC " +
                                    ",TSS.id_type_state_service idStus " +
                                    ",TSS.name [Status] " +
                                    ",COUNT(ADI.id_discount) " +
                                    ",CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name) " +
                                    ",A.type_agreement Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE CONCAT(UPPER(AD.street) , ' ' , UPPER(AD.outdoor)) LIKE '%" + search.StringSearch + "%' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), A.type_agreement, A.num_derivatives";
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    while (await result.ReadAsync())
                                    {
                                        clientsFilter.Add(new FindAgreementParamVM
                                        {
                                            AgreementId = Convert.ToInt32(result[0]),
                                            Account = result[1].ToString(),
                                            Nombre = result[2].ToString().ToUpper(),
                                            RFC = result[3].ToString(),
                                            idStus = Convert.ToInt32(result[4]),
                                            Status = result[5].ToString(),
                                            WithDiscount = Convert.ToBoolean(result[6]),
                                            Address = result[7].ToString(),
                                            Type = result[8].ToString(),
                                            NumDerivades =  Convert.ToInt32(result[9])
                                        });
                                    }
                                }
                            }
                        }
                        if (clientsFilter.Count > 0)
                            return Ok(clientsFilter);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    break;
                case 4:
                    search.StringSearch = search.StringSearch.ToUpper();

                    if (search.StringSearch.Length < 5)
                    {
                        return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No se pudo completar su busqueda, favor de ingresar un minimo de 5 caracteres para poder continuar" });
                    }
                    try
                    {
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT A.id_agreement " +
                                    ",A.account Account " +
                                    ",CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name) Nombre " +
                                    ",C.rfc RFC " +
                                    ",TSS.id_type_state_service idStus " +
                                    ",TSS.name [Status] " +
                                    ",COUNT(ADI.id_discount) " +
                                    ",CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name) " +
                                    ",A.type_agreement Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE UPPER(C.rfc) LIKE '%" + search.StringSearch + "%' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), A.type_agreement, A.num_derivatives";
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    while (await result.ReadAsync())
                                    {
                                        clientsFilter.Add(new FindAgreementParamVM
                                        {
                                            AgreementId = Convert.ToInt32(result[0]),
                                            Account = result[1].ToString(),
                                            Nombre = result[2].ToString().ToUpper(),
                                            RFC = result[3].ToString(),
                                            idStus = Convert.ToInt32(result[4]),
                                            Status = result[5].ToString(),
                                            WithDiscount = Convert.ToBoolean(result[6]),
                                            Address = result[7].ToString(),
                                            Type = result[8].ToString(),
                                            NumDerivades = Convert.ToInt32(result[9])
                                        });
                                    }
                                }
                            }
                        }
                        if (clientsFilter.Count > 0)
                            return Ok(clientsFilter);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    break;
                default:
                    break;
            }



            return BadRequest();
        }


        //PUT: api/Agreements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgreement([FromRoute] int id, [FromBody] AgreementVM agreementvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agreementvm.Id)
            {
                return BadRequest();
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Agreement agreement = await GetAgreementDataUpdate(agreementvm.Id);

                    AgreementLog log = new AgreementLog();
                    log.Agreement = agreement;
                    log.AgreementId = agreement.Id;
                    log.AgreementLogDate = DateTime.UtcNow.ToLocalTime();
                    log.Description = "Actualización de Datos";
                    log.Observation = agreementvm.Observations;
                    log.User = await userManager.FindByIdAsync(agreementvm.UserId);
                    log.UserId = agreementvm.UserId;
                    log.Visible = true;
                    log.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                    log.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    log.OldValue = JsonConvert.SerializeObject(agreement, new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                   if(agreement.AgreementDetails.Count > 0)
                    {
                        if(agreementvm.AgreementDetails.Count > 0)
                        {
                            var data = agreement.AgreementDetails.FirstOrDefault();
                            var newdata = agreementvm.AgreementDetails.FirstOrDefault();
                            data.AgreementDetailDate = newdata.AgreementDetailDate;
                            data.Built = newdata.Built;
                            data.Folio = newdata.Folio;
                            data.Ground = newdata.Ground;
                            data.LastUpdate = newdata.LastUpdate;
                            data.Observation = newdata.Observation;
                            data.Register = newdata.Register;
                            data.Sector = newdata.Sector;
                            data.TaxableBase = newdata.TaxableBase;
                        }
                      
                    }
                    var services = _context.AgreementServices.Where(xx => xx.IdAgreement == agreement.Id).ToList();
                    var ids = (from s in services
                               select s.IdService).ToList();

                    services.ForEach(x =>
                    {
                        if(agreementvm.ServicesId.Contains(x.IdService))
                        {
                            x.IsActive = true;
                            _context.Entry(x).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else
                        {
                            x.IsActive = false;
                            _context.Entry(x).State = EntityState.Modified;
                            _context.SaveChanges();
                        }

                    });

                    //var comparelist = ids.Except(agreementvm.ServicesId).ToList();
                    var comparelist = agreementvm.ServicesId.Except(ids).ToList();

                    comparelist.ForEach(x =>
                    {
                        _context.AgreementServices.AddAsync(new AgreementService
                        {
                            Agreement = agreement,
                            DateAgreement = DateTime.UtcNow.ToLocalTime(),
                            IdAgreement = agreement.Id,
                            IdService = x,
                            IsActive = true,
                            Service = _context.Services.Find(x)
                        });
                        _context.SaveChanges();
                    });

                    var service = await _context.TypeServices.FindAsync(agreementvm.TypeServiceId);
                    var intake = await _context.TypeIntakes.FindAsync(agreementvm.TypeIntakeId);
                    var use = await _context.TypeUses.FindAsync(agreementvm.TypeUseId);
                    var consume = await _context.TypeConsumes.FindAsync(agreementvm.TypeConsumeId);
                    var regime = await _context.TypeRegimes.FindAsync(agreementvm.TypeRegimeId);
                    var sService = await _context.TypeStateServices.FindAsync(agreementvm.TypeStateServiceId);
                    var period = await _context.TypePeriods.FindAsync(agreementvm.TypePeriodId);
                    var diam = await _context.Diameters.FindAsync(agreementvm.DiameterId);
                    var typeAgreement = await _context.Types.Where(z => z.CodeName == agreementvm.TypeAgreement).ToListAsync();

                    agreement.Route = agreementvm.Route;
                    agreement.TypeService = service;
                    agreement.TypeServiceId = service.Id;
                    agreement.TypeIntake = intake;
                    agreement.TypeIntakeId = intake.Id;
                    agreement.TypeUse = use;
                    agreement.TypeUseId = use.Id;
                    agreement.TypeConsume = consume;
                    agreement.TypeConsumeId = consume.Id;
                    agreement.TypeRegime = regime;
                    agreement.TypeRegimeId = regime.Id;
                    agreement.TypeStateService = sService;
                    agreement.TypeStateServiceId = sService.Id;
                    agreement.TypePeriod = period;
                    agreement.TypePeriodId = period.Id;
                    agreement.Diameter = diam;
                    agreement.DiameterId = diam.Id;

                    log.NewValue = JsonConvert.SerializeObject(agreement,new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    _context.AgreementLogs.Add(log);
                    _context.SaveChanges();


                    _context.Entry(agreement).State = EntityState.Modified;
                    _context.SaveChanges();
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
                systemLog.Parameter = JsonConvert.SerializeObject(agreementvm);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el contrato" });
            }
            return NoContent();
        }



        // POST: api/Agreements
        [HttpPost]
        public async Task<IActionResult> PostAgreement([FromBody] AgreementVM agreementvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           

            TypeCommercialBusiness cBusiness = null;
            Agreement NewAgreement = new Agreement();
            Agreement Principal = null;
            bool IsDerivative = false;
            bool HasError = false;

            if (agreementvm.TypeCommertialBusinessId == 0)
            {
                cBusiness = await _context.TypeCommertialBusinesses.FindAsync(1);
            }
            else
            {
                cBusiness = await _context.TypeCommertialBusinesses.FindAsync(agreementvm.TypeCommertialBusinessId);
            }
            var service = await _context.TypeServices.FindAsync(agreementvm.TypeServiceId);
            var intake = await _context.TypeIntakes.FindAsync(agreementvm.TypeIntakeId);
            var use = await _context.TypeUses.FindAsync(agreementvm.TypeUseId);
            var consume = await _context.TypeConsumes.FindAsync(agreementvm.TypeConsumeId);
            var regime = await _context.TypeRegimes.FindAsync(agreementvm.TypeRegimeId);
            var sService = await _context.TypeStateServices.FindAsync(agreementvm.TypeStateServiceId);
            var period = await _context.TypePeriods.FindAsync(agreementvm.TypePeriodId);
            var diam = await _context.Diameters.FindAsync(agreementvm.DiameterId);
            var typeAgreement = await _context.Types.Where(z => z.CodeName == agreementvm.TypeAgreement).ToListAsync();
            var typeClas = await _context.TypeClassifications.FindAsync(agreementvm.TypeClasificationId);
            if (service == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest, 
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de servicio')]" });
            }
            if (intake == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de toma')]" });
            }
            if (use == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de uso')]" });
            }
            if (consume == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de consumo')]" });
            }
            if (regime == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de regimen')]" });
            }
            if (cBusiness == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de negocio comercial')]" });
            }
            if (sService == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo estado del servicio')]" });
            }
            if (period == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de periodo')]" });
            }
            if (diam == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de diametro')]" });
            }
            if (agreementvm.ServicesId.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Debe agregar por lo menos un servio al contrato')]" });
            }
            if (agreementvm.Adresses.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Debe agregar por lo menos una dirección al contrato')]" });
            }
            if (agreementvm.Clients.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Debe agregar por lo menos un cliente al contrato')]" });
            }
            if(typeAgreement == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Debe verificar el tipo de contrato (Principal / Derivado)')]" });
            }

            if (typeClas == null)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "Se ha enviado mal los datos favor de verificar [Detalles('Problemas en el tipo de Tipo de clasificación')]" });
            }

            if (service != null && intake != null && use != null
                               && consume != null && regime != null
                               && cBusiness != null && sService != null
                               && period != null && diam != null && typeClas != null
                               && agreementvm.ServicesId.Count > 0
                               && agreementvm.Adresses.Count > 0
                               && agreementvm.Clients.Count > 0)

            {

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if(await _context.Agreements.Where(x => x.Account == agreementvm.Account).FirstOrDefaultAsync() != null)
                        {
                            return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "El número de cuenta ya fue asignado a otro contrato, Favor de verificar " });
                        }

                        if(agreementvm.AgreementPrincipalId != 0)
                        {
                            Principal = await _context.Agreements.Include(a => a.Addresses)
                                                                    .ThenInclude(s => s.Suburbs)
                                                                 .Where(x => x.Id == agreementvm.AgreementPrincipalId)
                                                                 .FirstOrDefaultAsync();
                            if(Principal.TypeAgreement == "AGR02")
                            {
                                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "El número de cuenta es un contrato derivado, no se puede realizar esta operación, Favor de verificar " });
                            }
                        }

                        NewAgreement.Account = agreementvm.Account;
                        //NewAgreement.AccountDate = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                        NewAgreement.AccountDate = DateTime.UtcNow.ToLocalTime();
                        NewAgreement.StratDate = DateTime.UtcNow.ToLocalTime();
                        NewAgreement.NumDerivatives = agreementvm.Derivatives;
                        NewAgreement.Route = agreementvm.Route;
                        NewAgreement.TypeAgreement = agreementvm.TypeAgreement;
                        NewAgreement.TypeService = service;
                        NewAgreement.TypeIntake = intake;
                        NewAgreement.TypeUse = use;
                        NewAgreement.TypeConsume = consume;
                        NewAgreement.TypeRegime = regime;
                        NewAgreement.TypeStateService = sService;
                        NewAgreement.TypePeriod = period;
                        NewAgreement.TypeCommertialBusiness = cBusiness;
                        NewAgreement.Diameter = diam;
                        NewAgreement.TypeClassification = typeClas;

                        //
                        if (Principal != null)
                        {

                            foreach (var item in agreementvm.Adresses)
                            {
                                if(item.TypeAddress == "DIR01")
                                {
                                    var suburb = await _context.Suburbs.FindAsync(item.SuburbsId);
                                    if (Principal.Addresses.Where(p => p.TypeAddress == "DIR01").FirstOrDefault().Suburbs.Name != suburb.Name)
                                    {
                                        HasError = true;
                                    }
                                    else
                                    {
                                        Principal.NumDerivatives = Principal.NumDerivatives + 1;
                                        _context.Entry(Principal).State = EntityState.Modified;
                                        await _context.SaveChangesAsync();
                                        IsDerivative = true;
                                    }
                                }
                            }

                            if (HasError)
                            {
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El contrato no puede ser derivada, ya que no coincide la dirección o la colonia " });
                            }

                        }

                        foreach (var address in agreementvm.Adresses)
                        {
                            NewAgreement.Addresses.Add(new Address
                            {
                                Street = address.Street,
                                Outdoor = address.Outdoor,
                                Indoor = address.Indoor,
                                Zip = address.Zip,
                                Reference = address.Reference,
                                Lat = address.Lat,
                                Lon = address.Lon,
                                TypeAddress = address.TypeAddress,
                                Suburbs = await _context.Suburbs.FindAsync(address.SuburbsId),
                                IsActive = true
                            });
                        }

                        foreach (var client in agreementvm.Clients)
                        {
                            Client nc = new Client()
                            {
                                Name = client.Name,
                                LastName = client.LastName,
                                SecondLastName = client.SecondLastName,
                                RFC = (client.RFC == "") ? "XAXX010101000" : client.RFC,
                                CURP = (client.CURP == "") ? (client.IsMale == true) ? "XEXX010101HNEXXXA4" : "XEXX010101HNEXXXA8" : client.CURP,
                                INE = client.INE,
                                EMail = client.EMail,
                                TypeUser = client.TypeUser,
                                IsActive = true
                            };

                            foreach (var item in client.Contacts)
                            {
                                nc.Contacts.Add(new Contact
                                {
                                    PhoneNumber = item.PhoneNumber,
                                    TypeNumber = item.TypeNumber,
                                    IsActive = 1
                                });
                            }
                            NewAgreement.Clients.Add(nc);
                        }

                        await _context.Agreements.AddAsync(NewAgreement);
                        await _context.SaveChangesAsync();
                        if (IsDerivative)
                        {
                            Derivative derivative = new Derivative()
                            {
                                Agreement = Principal,
                                AgreementId = Principal.Id,
                                AgreementDerivative = NewAgreement.Id,
                                IsActive = true
                            };
                            //_context.Derivatives.Attach(derivative);
                            await _context.Derivatives.AddAsync(derivative);
                            await _context.SaveChangesAsync();

                            AgreementLog agreementLogderivative = new AgreementLog()
                            {
                                Agreement = NewAgreement,
                                AgreementLogDate = DateTime.UtcNow.ToLocalTime(),
                                AgreementId = NewAgreement.Id,
                                UserId = agreementvm.UserId,
                                User = await userManager.FindByIdAsync(agreementvm.UserId),
                                Description = "Se Agrego Derivada al Contrato con Cuenta " + Principal.Account,
                                Observation = agreementvm.Observations,
                                NewValue = "",
                                OldValue = "",
                                Visible = false,
                                Action = "PostContoller",
                                Controller = "AgreementController"
                            };
                            
                            await _context.AgreementLogs.AddAsync(agreementLogderivative);
                            int a  = await _context.SaveChangesAsync();
                        }
                        

                        foreach (var aservice in agreementvm.ServicesId)
                        {
                            await _context.AgreementServices.AddAsync(new AgreementService
                            {
                                Agreement = NewAgreement,
                                DateAgreement = DateTime.UtcNow.ToLocalTime(),
                                IdAgreement = NewAgreement.Id,
                                IdService = aservice,
                                IsActive = true,
                                Service = await _context.Services.FindAsync(aservice)
                            });
                            await _context.SaveChangesAsync();
                        }
                        if (!IsDerivative)
                        {
                            AgreementLog agreementLog = new AgreementLog()
                            {
                                Agreement = NewAgreement,
                                AgreementLogDate = DateTime.UtcNow.ToLocalTime(),
                                User = await userManager.FindByIdAsync(agreementvm.UserId),
                                UserId = agreementvm.UserId,
                                Description = "Nuevo Contrato",
                                Observation = agreementvm.Observations,
                                NewValue = "",
                                OldValue = "",
                                Visible = false,
                                Action = "PostContoller",
                                Controller = "AgreementController",
                            };
                            await _context.AgreementLogs.AddAsync(agreementLog);
                        }
                       


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
                    systemLog.Parameter = JsonConvert.SerializeObject(agreementvm);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el contrato" });
                }
            }
            RedirectToActionResult redirect = new RedirectToActionResult("GetAccountById", "Agreements", new { @id = NewAgreement.Id });
            return redirect;
        }

        [HttpGet(Name = "GetAccountById")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var data = _context.Agreements.Find(id);
            return StatusCode((int)TypeError.Code.Ok, new { Success = "El número de cuenta asignado fue: " + data.Account });
        }


      



        [HttpPost("AddDiscount")]
        public async Task<IActionResult> AddDiscount([FromBody]  AgreementDiscounttVM agreementDiscountt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Agreement agreement = await _context.Agreements
                                                .Include(x => x.TypeIntake)
                                                .Include(x => x.TypeStateService)
                                                //.Include(x => x.AgreementDiscounts.Where(z => z.IsActive == true))
                                                .Where(x => x.Id == agreementDiscountt.AgreementId)
                                                .FirstOrDefaultAsync();
            agreement.AgreementDiscounts = await _context.AgreementDiscounts.Where(x => x.IsActive == true && x.IdAgreement == agreementDiscountt.AgreementId).ToListAsync();
            Discount discount = await _context.Discounts.FindAsync(agreementDiscountt.DiscountId);


            if (agreement == null || discount == null)
            { 
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "El número de contrato o El tipo de descuento no se no se encuentran, favor de verificar" });
            }

            if(agreement.TypeIntake.Acronym != "HA")
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
            }

            if(agreement.TypeStateService.Id != 1 && agreement.TypeStateService.Id != 3)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
            }

            if(agreement.AgreementDiscounts.Count >= 1)//suspendido
            {
                //if(agreement.AgreementDiscounts.)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El contrato no permite asignar más de un descuento, favor de verificar" });
            }

            if (discount.IsActive == false)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El descuento no se encuentra activo, favor de verificar" });
            }

            if (discount.EndDate.Value < DateTime.UtcNow.ToLocalTime())
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El descuento no se encuentra dentro de un periodo valido, favor de verificar" });
            }
          

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    AgreementDiscount agreementDiscount = new AgreementDiscount();
                    agreementDiscount.Agreement = agreement;
                    agreementDiscount.Discount = discount;
                    agreementDiscount.IdAgreement = agreement.Id;
                    agreementDiscount.IdDiscount = discount.Id;
                    agreementDiscount.IsActive = true;
                    agreementDiscount.StartDate = DateTime.UtcNow.ToLocalTime();
                    agreementDiscount.EndDate = DateTime.UtcNow.ToLocalTime().AddMonths(discount.Month);

                    await _context.AgreementDiscounts.AddAsync(agreementDiscount);
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
                systemLog.Parameter = JsonConvert.SerializeObject(agreementDiscountt);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el descuento" });

            }
            return Ok();
        }

        [HttpPut("PutDiscount/{AgreementId}")]
        public async Task<IActionResult> PutDiscounts([FromRoute]int AgreementId, [FromBody]  AgreementDiscounttVM agreementDiscountt)
        {
           
            if (AgreementId != agreementDiscountt.AgreementId)
            {
                return BadRequest();
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    AgreementDiscount agreementd = await _context.AgreementDiscounts
                                                       .Where(x => x.IdAgreement == agreementDiscountt.AgreementId
                                                                && x.IdDiscount == agreementDiscountt.DiscountId)
                                                       .FirstOrDefaultAsync();
                    agreementd.IsActive = agreementDiscountt.IsActive;

                    AgreementLog log = new AgreementLog()
                    {
                        Action = this.ControllerContext.RouteData.Values["action"].ToString(),
                        Controller = this.ControllerContext.RouteData.Values["controller"].ToString(),
                        Observation = agreementDiscountt.Observation,
                        OldValue = JsonConvert.SerializeObject(agreementDiscountt),
                        User = await userManager.FindByIdAsync(agreementDiscountt.UserId),
                        UserId = agreementDiscountt.UserId,
                        Visible = true,
                        AgreementId = agreementDiscountt.AgreementId,
                        Agreement = await _context.Agreements.FindAsync(agreementDiscountt.AgreementId),
                        AgreementLogDate = DateTime.UtcNow.ToLocalTime(),
                        Description = "Actualización de Descuentos",
                        NewValue = ""
                    };
                   
                    _context.Entry(agreementd).State = EntityState.Deleted;
                    _context.AgreementLogs.Add(log);
                    _context.SaveChanges();
                    scope.Complete();
                }
            }
            catch(Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(agreementDiscountt);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el descuento" });
            }
            //agreement.
            //if()
            return Ok();
        }

        [HttpGet("GetDiscounts/{AgreementId}")]
        public async Task<IActionResult> GetDiscounts([FromRoute]  int AgreementId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var valueSystem =  await _context.SystemParameters.Where(x => x.Name == "ISMUNICIPAL").FirstOrDefaultAsync();


            if(valueSystem.TextColumn == "NO")
            {
                Agreement agreement = await _context.Agreements
                                               .Include(x => x.TypeIntake)
                                               .Include(x => x.TypeStateService)
                                               //.Include(x => x.AgreementDiscounts.Where(z => z.IsActive == true))
                                               .Where(x => x.Id == AgreementId)
                                               .FirstOrDefaultAsync();





                if (agreement.TypeIntake.Acronym != "HA")
                {
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
                }

                if (agreement.TypeStateService.Id != 1 && agreement.TypeStateService.Id != 3)
                {
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
                }


                var discounts = await _context.AgreementDiscounts

                                              .Include(x => x.Discount)
                                              .Where(x => x.IdAgreement == AgreementId &&
                                                          x.IsActive == true).ToListAsync();

                return Ok(discounts);
            }
            else
            {
                Agreement agreement = await _context.Agreements
                                             .Include(x => x.TypeIntake)
                                             .Include(x => x.TypeStateService)
                                             .Where(x => x.Id == AgreementId)
                                             .FirstOrDefaultAsync();

                var catalogue = await _context.GroupCatalogues.Include(x => x.Catalogues).Where(x => x.Id == agreement.TypeUse.Id).FirstOrDefaultAsync();

                if(catalogue == null)
                {
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });

                }

                var discounts = await _context.AgreementDiscounts

                                             .Include(x => x.Discount)
                                             .Where(x => x.IdAgreement == AgreementId &&
                                                         x.IsActive == true).ToListAsync();

                return Ok(discounts);

            }



        }



        // DELETE: api/Agreements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgreement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agreement = await _context.Agreements.FindAsync(id);
            if (agreement == null)
            {
                return NotFound();
            }

            _context.Agreements.Remove(agreement);
            await _context.SaveChangesAsync();

            return Ok(agreement);
        }

        [HttpPost("AddMeter")]
        public async Task<IActionResult> AddMeter([FromBody] MeterVM meterVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Meter meter = new Meter();
            var agreement = await _context.Agreements.FindAsync(meterVM.AgreementId);
            if (agreement != null)
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        meter.Agreement = agreement;
                        meter.Brand = meterVM.Brand;
                        meter.Consumption = meterVM.Consumption;
                        meter.DeinstallDate = meter.DeinstallDate;
                        meter.InstallDate = meterVM.InstallDate;
                        meter.IsActive = meterVM.IsActive;
                        meter.Model = meterVM.Model;
                        meter.Serial = meterVM.Serial;
                        meter.Wheels = meterVM.Wheels;

                        await _context.Meters.AddAsync(meter);
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
                    systemLog.Parameter = JsonConvert.SerializeObject(meterVM);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                }
            }
            else
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Se ha enviado mal los datos favor de verificar" });
            }
            return Ok(meter);
        }

        //[HttpGet("AgrementDetailDiscount/{AgreementId}")]
        //public async Task<IActionResult> GetAgrementDetailDiscount([FromRoute] int AgreementId)
        //{
        //    var agreement = await _context.Agreements
        //                            .Include(x => x.AgreementDetails)
        //                            .Include(ad => ad.AgreementDiscounts)
        //                            .Where(x => x.Id == AgreementId)
        //                            .FirstOrDefaultAsync();
        //    return Ok(agreement);
        //}

        [HttpGet("GetData")]
        public async Task<IActionResult> GetInitialAgreements()
        {
            return Ok(new AgreementDataVM()
            {
                TypeService = await _context.TypeServices.Where(a => a.IsActive == true).ToListAsync(),
                TypeIntake = await _context.TypeIntakes.Where(a => a.IsActive == true).ToListAsync(),
                TypeUse = await _context.TypeUses.Where(a => a.IsActive == true).ToListAsync(),
                TypeConsume = await _context.TypeConsumes.Where(a => a.IsActive == true).ToListAsync(),
                TypeRegime = await _context.TypeRegimes.Where(a => a.IsActive == true).ToListAsync(),
                TypeCommertialBusiness = await _context.TypeCommertialBusinesses.Where(a => a.IsActive == true).ToListAsync(),
                TypeStateService = await _context.TypeStateServices.Where(a => a.IsActive == true).ToListAsync(),
                TypePeriod = await _context.TypePeriods.Where(a => a.IsActive == true).ToListAsync(),
                Diameter = await _context.Diameters.Where(a => a.IsActive == true).ToListAsync(),
                TypeClassifications = await _context.TypeClassifications.Where(a => a.IsActive == true).ToListAsync(),
                TypeAddresses = await _context.Types.Where(x => x.GroupType.Id == 1)
                                                    .Select(n => new TypeAddress()
                                                    {
                                                        IdType = n.CodeName,
                                                        Description = n.Description
                                                    }).ToListAsync(),
                TypeClients = await _context.Types.Where(x => x.GroupType.Id == 2)
                                                    .Select(n => new TypeClient()
                                                    {
                                                        IdType = n.CodeName,
                                                        Description = n.Description
                                                    }).ToListAsync(),
                TypeContacts = await _context.Types.Where(x => x.GroupType.Id == 3)
                                                    .Select(n => new TypeContact()
                                                    {
                                                        IdType = n.CodeName,
                                                        Description = n.Description
                                                    }).ToListAsync(),
                TypeAgreemnets = await _context.Types.Where(x => x.GroupType.Id == 4)
                                                .Select(n => new TypeAgreemnet()
                                                {
                                                    IdType = n.CodeName,
                                                    Description = n.Description
                                                }).ToListAsync(),

                TypeDescounts = await _context.Discounts.Where(x => x.IsActive == true)
                                                        .Select(d => new TypeDiscount()
                                                        {
                                                            IdType = d.Id,
                                                            Description = d.Name,
                                                            Percentage = d.Percentage
                                                        }).ToListAsync(),
                TypeDebts = await _context.Types.Where(x => x.GroupTypeId == 5)
                                                .Select(n => new TypeDebt()
                                                {
                                                    IdType = n.CodeName,
                                                    Description = n.Description
                                                }).ToListAsync(),

                TypeFile = await _context.Types.Where(x => x.GroupTypeId == 9)
                                                .Select(n => new TypeFile()
                                                {
                                                    IdType = n.CodeName,
                                                    Description = n.Description
                                                }).ToListAsync(),

                Services = await _context.Services
                                        .Where(s => s.IsActive == true && s.InAgreement == true)
                                        .Select(s => new ServiceVM
                                        {
                                            Id = s.Id,
                                            Name = s.Name
                                        }).ToListAsync()
            });
        }

        private async Task<Agreement> GetAgreementData(int id)
        {
            var agreemet = await _context.Agreements
                                      .Include(x => x.Clients)
                                        .ThenInclude(contact => contact.Contacts)
                                      .Include(x => x.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                      .Include(ts => ts.TypeService)
                                      .Include(tu => tu.TypeUse)
                                      .Include(tc => tc.TypeConsume)
                                      .Include(tr => tr.TypeRegime)
                                      .Include(tp => tp.TypePeriod)
                                      .Include(tcb => tcb.TypeCommertialBusiness)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ti => ti.TypeIntake)
                                      .Include(di => di.Diameter)
                                      .Include(tc => tc.TypeClassification)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .Include(ad => ad.AgreementDiscounts)
                                        .ThenInclude(d => d.Discount)
                                      .Include(ad => ad.AgreementDetails)
                                      .Include(af => af.AgreementFiles)
                                      .FirstOrDefaultAsync(a => a.Id == id);

            agreemet.Addresses.ToList().ForEach(x =>
            {
                x.Suburbs = _context.Suburbs.Include(r => r.Regions)
                                            .Include(c => c.Clasifications)
                                            .Include(t => t.Towns)
                                                .ThenInclude(s => s.States)
                                                .ThenInclude(c => c.Countries)
                                            .Where(i => i.Id == x.Suburbs.Id)
                                            .SingleOrDefault();
            });
            var service = agreemet.AgreementServices.Where(x => x.IsActive == false);
            agreemet.AgreementServices = agreemet.AgreementServices.Except(service).ToList();
            agreemet.Clients.ToList().ForEach(x =>
            {
                x.Contacts = x.Contacts.Where(a => a.IsActive == 1).ToList();
            });

            return agreemet;
        }

        private async Task<Agreement> GetAgreementDataUpdate(int id)
        {
            var agreemet = await _context.Agreements
                                      .Include(x => x.Clients)
                                        .ThenInclude(contact => contact.Contacts)
                                      .Include(x => x.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                      .Include(ts => ts.TypeService)
                                      .Include(tu => tu.TypeUse)
                                      .Include(tc => tc.TypeConsume)
                                      .Include(tr => tr.TypeRegime)
                                      .Include(tp => tp.TypePeriod)
                                      .Include(tcb => tcb.TypeCommertialBusiness)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ti => ti.TypeIntake)
                                      .Include(di => di.Diameter)
                                      .Include(tc => tc.TypeClassification)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .Include(ad => ad.AgreementDiscounts)
                                        .ThenInclude(d => d.Discount)
                                      .Include(ad => ad.AgreementDetails)
                                      .Include(af => af.AgreementFiles)
                                      .FirstOrDefaultAsync(a => a.Id == id);

            agreemet.Addresses.ToList().ForEach(x =>
            {
                x.Suburbs = _context.Suburbs.Include(r => r.Regions)
                                            .Include(c => c.Clasifications)
                                            .Include(t => t.Towns)
                                                .ThenInclude(s => s.States)
                                                .ThenInclude(c => c.Countries)
                                            .Where(i => i.Id == x.Suburbs.Id)
                                            .SingleOrDefault();
            });
            //var service = agreemet.AgreementServices.Where(x => x.IsActive == false);
            //agreemet.AgreementServices = agreemet.AgreementServices.Except(service).ToList();

            return agreemet;
        }
        private bool AgreementExists(int id)
        {
            return _context.Agreements.Any(e => e.Id == id);
        }

        [HttpGet("getRoutesFromAgreement")]
        public async Task<IActionResult> getRoutesFromAgreement()
        {
            char[] caracteres = { '}', '{', '?', '!', '"', '#', '$', '%', '&', '/', '(', ')', '=', '¿', '¡', '\'', '\\', '*', '[', ']', '-', '_', ' ', '.', ':', ',', ';', '´', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'ñ', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            try
            {                
                var res = _context.Agreements.Where(x => !x.Route.Intersect(caracteres).Any() && !string.IsNullOrEmpty(x.Route)).Select(y => int.Parse(y.Route)).OrderBy(z => z).Distinct().ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}