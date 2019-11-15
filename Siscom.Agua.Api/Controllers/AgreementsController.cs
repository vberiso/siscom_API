using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
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
using Siscom.Agua.Api.runSp;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.Security;
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
        [HttpGet("GetSummary/{AgreementId}/{ById?}")]
        public async Task<IActionResult> GetAgreements([FromRoute] int AgreementId, [FromRoute] int ById = 0)
        {
            var query = _context.Agreements
                                    .Include(a => a.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                            .ThenInclude(t => t.Towns)
                                                .ThenInclude(st => st.States)
                                    .Include(c => c.Clients)
                                    .Include(ti => ti.TypeIntake)
                                    .Include(ts => ts.TypeStateService)
                                    .Include(sd => sd.TypeService)
                                    .Include(tc => tc.TypeConsume)
                                    .Include(ad => ad.AgreementDetails)
                                    .Include(di => di.AgreementDiscounts)
                                        .ThenInclude(d => d.Discount)
                                    .Include(p => p.Prepaids);
            Siscom.Agua.DAL.Models.Agreement summary = null;
            if (ById == 1)
            {
                summary = await query.Where(a => a.Id == AgreementId).FirstOrDefaultAsync();
            }
            else
            {
                summary = await query.Where(a => a.Account == AgreementId.ToString()).FirstOrDefaultAsync();
            }


            summary.Clients = summary.Clients.Where(c => c.IsActive == true).ToList();
            summary.Addresses = summary.Addresses.Where(a => a.TypeAddress == "DIR01" && a.IsActive == true).ToList();

            var status = await _context.Statuses.ToListAsync();
            var type = await _context.Types.ToListAsync();

            summary.Prepaids.ToList().ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Status))
                    x.StatusDescription = (from d in status
                                           where d.CodeName == x.Status
                                           select d).FirstOrDefault().Description;

                if (!string.IsNullOrEmpty(x.Type))
                    x.TypeDescription = (from d in type
                                         where d.CodeName == x.Type
                                         select d).FirstOrDefault().Description;
            });


            //summary.Debts = await _context.Debts
            //                              .Include(gs => gs.DebtDetails)
            //                              .Where(gs => _context.Statuses
            //                              .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == summary.Id).ToListAsync();
            summary.Debts = await _context.Debts
                                          .Include(ddis => ddis.DebtDiscounts)
                                          .Include(gs => gs.DebtDetails)
                                          .Where(gs => _context.Statuses
                                          .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == summary.Id).ToListAsync();

            return Ok(summary);
        }


        // GET: api/Agreements/5
        [HttpGet("{id}", Name = "GetAgreementById")]
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



        [HttpGet("comparateAccount/{account}/{idIntake}")]
        public async Task<IActionResult> CompareteAccount([FromRoute] string account, int idIntake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var agreement = _context.Agreements
                                        .Include(c => c.TypeIntake)
                                        .Where(x => x.Account == account).FirstOrDefault();


            if (agreement == null)
            {
                return Ok("Numero de cuenta valido: " + account);
            }
            else
            {
                if (agreement.TypeIntake.Id == idIntake)
                {

                    return NotFound("Ya existe esta numero de cuenta con tipo " + agreement.TypeIntake.Name);
                }
                else
                {
                    return Ok("Numero de cuenta valido: " + account);

                }

            }



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
        [HttpGet("AgreementByAccount/Cash/{AcountNumber}/{agreementID?}")]
        public async Task<IActionResult> GetGetAgreementByAccountCash([FromRoute] string AcountNumber, [FromRoute] string agreementID = "")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var query = _context.Agreements
                                     .Include(x => x.Clients)
                                     .Include(x => x.TypeStateService)
                                     .Include(x => x.AgreementDiscounts)
                                     .Include(x => x.AgreementDetails)
                                     .Include(x => x.TypeIntake)
                                     .Include(x => x.Addresses)
                                       .ThenInclude(s => s.Suburbs)
                                           .ThenInclude(t => t.Towns)
                                               .ThenInclude(st => st.States)
                                     .Include(x => x.AgreementComments)
                                     .Include(x => x.PartialPayments);

                List<Siscom.Agua.DAL.Models.Agreement> agreement;

                if (string.IsNullOrEmpty(agreementID))
                {
                    agreement = await query.Where(a => a.Account == AcountNumber).ToListAsync();
                }
                else
                {
                    agreement = await query.Where(a => a.Id == Convert.ToInt32(agreementID)).ToListAsync();
                }


                if (agreement == null)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "No hay datos para este número de cuenta" });
                }
                agreement.ForEach(x =>
                {
                    x.Clients = x.Clients.Where(c => c.TypeUser == "CLI01" && c.IsActive == true).ToList();
                    x.Addresses = x.Addresses.ToList();
                    x.AgreementComments = x.AgreementComments.Where(ac => ac.IsVisible == true).ToList();

                });

                ;

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
                                      .Include(a => a.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                         .ThenInclude(t => t.Towns)
                                            .ThenInclude(st => st.States)
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


        [HttpGet("AgreementsDerivate/{account}")]
        public async Task<IActionResult> GetAgreementsDerivate([FromRoute] string account)
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
                                      .Include(a => a.Addresses)
                                        .ThenInclude(s => s.Suburbs)
                                         .ThenInclude(t => t.Towns)
                                            .ThenInclude(st => st.States)
                                      .Include(tc => tc.TypeClassification)
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .Include(ad => ad.AgreementDetails)
                                      .Include(af => af.AgreementFiles)
                                      .FirstOrDefaultAsync(a => a.Account == account);


            if (agreement == null)
            {
                return NotFound("No se encontro contrato");
            }
            return Ok(agreement);
        }

        [HttpGet("Notification/{idAgreement}")]
        public async Task<IActionResult> GetNotification([FromRoute] int idAgreement)
        {

            var sDate = DateTime.Now.ToString();
            var datevalue = (Convert.ToDateTime(sDate.ToString()));

            //DateTime dy = datevalue.Day.ToString();
            var mn = datevalue.Month.ToString();
            var yy = datevalue.Year.ToString();

            int year = 0;

            int month = 0;

            year = Convert.ToInt32(yy);
            List<ConstanciaVm> pi = new List<ConstanciaVm>();

            month = Convert.ToInt32(mn);

            month = month - 2;




            var agreement = await _context.Debts.Include(d => d.DebtDetails)
                                                .Where(a => a.AgreementId == idAgreement && a.Status != "ED005" && a.ExpirationDate.Year == year).OrderByDescending(e => e.DebitDate).ToListAsync();

            if (agreement == null || agreement.Count == 0)
            {
                var pay = await _context.Payments.Include(p => p.PaymentDetails).Where(pa => pa.AgreementId == idAgreement && pa.PaymentDate.Year == year && pa.PaymentDate.Month == month).ToListAsync();

                if (pay == null)
                {
                    return NotFound("no existen pagos");
                }

                //var deta = await _context.PaymentDetails.Where(xa => xa.PaymentId == pay.Id ).ToListAsync();



                foreach (var payment in pay)
                {

                    foreach (var detail in payment.PaymentDetails)
                    {
                        if (detail.Description == "Constancia de No Adeudo")
                        {

                            pi.Add(new ConstanciaVm()
                            {
                                Id = detail.Id,
                                Description = detail.Description
                            });

                        }
                    }


                }

                if (pi == null || pi.Count == 0)
                {
                    return NotFound("No tiene la constancia de no adeudo");
                }
                else
                {
                    return Ok("Obtener constancia de no adeudo");
                }


            }
            else
            {
                return NotFound("No contiene una constancia de no adeudo");
            }







        }

        [HttpGet("GetDerivatives/{id}")]
        public async Task<IActionResult> GetDerivatives([FromRoute] int id)
        {

            List<DerivativesVM> dero = new List<DerivativesVM>();
            var deriv = await _context.Derivatives
                                            .Include(x => x.Agreement)
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
                return StatusCode((int)TypeError.Code.BadRequest,
                                                  new { Error = "No tiene derivadas" });
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
                return StatusCode((int)TypeError.Code.BadRequest,
                                                                  new { Error = "No tiene derivadas" });
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
                                    ",TY.name Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    ",(Select ISNULL(Sum(D.amount - D.on_account),0) from Debt D Where D.AgreementId = A.id_agreement AND D.status in (Select St.id_status from Status St Where St.GroupStatusId = 4)) Debit " +
                                    ",A.token " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "INNER JOIN [dbo].Type_Intake AS TY ON TY.id_type_intake= A.TypeIntakeId " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE A.account = '" + search.StringSearch + "' AND AD.type_address = 'DIR01' AND C.type_user = 'CLI01' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), TY.name, A.num_derivatives, A.token";
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
                                            NumDerivades = Convert.ToInt32(result[9]),
                                            Debit = Convert.ToInt32(result[10]),
                                            Token = result[11].ToString()
                                        });
                                    }
                                }
                                //clientsFilter.ForEach(x =>
                                //{
                                //    x.Type = _context.Types.Where(y => y.CodeName == x.Type).FirstOrDefault().Description;
                                //});
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
                    if (search.StringSearch.Length < 2)
                    {
                        return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No se pudo completar su busqueda, favor de ingresar un minimo de 2 caracteres para poder continuar" });
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
                                    ",TY.name Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    ",(Select ISNULL(Sum(D.amount - D.on_account),0) from Debt D Where D.AgreementId = A.id_agreement AND D.status in (Select St.id_status from Status St Where St.GroupStatusId = 4)) Debit " +
                                    ",A.token " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "INNER JOIN [dbo].Type_Intake AS TY ON TY.id_type_intake= A.TypeIntakeId " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE CONCAT(UPPER(C.name) , ' ' , UPPER(C.last_name), ' ' , UPPER(C.second_last_name)) LIKE '%" + search.StringSearch + "%' AND AD.type_address = 'DIR01' AND C.type_user = 'CLI01' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), TY.name, A.num_derivatives, A.token";
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
                                            NumDerivades = Convert.ToInt32(result[9]),
                                            Debit = Convert.ToInt32(result[10]),
                                            Token = result[11].ToString()
                                        });
                                    }
                                }

                                //clientsFilter.ForEach(x =>
                                //{
                                //    x.Type = _context.Types.Where(y => y.CodeName == x.Type).FirstOrDefault().Description;
                                //});
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
                                    ",TY.name Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    ",(Select ISNULL(Sum(D.amount - D.on_account),0) from Debt D Where D.AgreementId = A.id_agreement AND D.status in (Select St.id_status from Status St Where St.GroupStatusId = 4)) Debit " +
                                    ",A.token " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "INNER JOIN [dbo].Type_Intake AS TY ON TY.id_type_intake= A.TypeIntakeId " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE CONCAT(UPPER(AD.street) , ' ' , UPPER(AD.outdoor)) LIKE '%" + search.StringSearch + "%' AND AD.type_address = 'DIR01' AND C.type_user = 'CLI01' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), TY.name, A.num_derivatives, A.token";
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
                                            NumDerivades = Convert.ToInt32(result[9]),
                                            Debit = Convert.ToInt32(result[10]),
                                            Token = result[11].ToString()
                                        });
                                    }
                                }
                                //clientsFilter.ForEach(x =>
                                //{
                                //    x.Type = _context.Types.Where(y => y.CodeName == x.Type).FirstOrDefault().Description;
                                //});
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
                                    ",TY.name Tipo" +
                                    ",A.num_derivatives Derivadas " +
                                    ",(Select ISNULL(Sum(D.amount - D.on_account),0) from Debt D Where D.AgreementId = A.id_agreement AND D.status in (Select St.id_status from Status St Where St.GroupStatusId = 4)) Debit " +
                                    ",A.token " +
                                    "FROM [dbo].[Client] as C " +
                                    "INNER JOIN [dbo].[Agreement] AS A ON C.AgreementId = A.id_agreement " +
                                    "INNER JOIN [dbo].[Address] AS AD ON C.AgreementId = AD.AgreementsId " +
                                    "INNER JOIN [dbo].[Type_State_Service] AS TSS ON A.TypeStateServiceId = TSS.id_type_state_service " +
                                    "INNER JOIN [dbo].Type_Intake AS TY ON TY.id_type_intake= A.TypeIntakeId " +
                                    "LEFT JOIN [dbo].[Agreement_Discount] AS ADI ON C.AgreementId = ADI.id_agreement " +
                                    "INNER JOIN [dbo].[Suburb] AS S ON AD.SuburbsId = S.id_suburb " +
                                    "WHERE UPPER(C.rfc) LIKE '%" + search.StringSearch + "%' AND AD.type_address = 'DIR01' AND C.type_user = 'CLI01' " +
                                    "GROUP BY A.id_agreement, A.account, CONCAT(C.name , ' ' , c.last_name, ' ' , C.second_last_name), RFC, TSS.id_type_state_service, TSS.name, CONCAT(AD.street, ' ', AD.outdoor, ' ', S.name), TY.name, A.num_derivatives, A.token";
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
                                            NumDerivades = Convert.ToInt32(result[9]),
                                            Debit = Convert.ToInt32(result[10]),
                                            Token = result[11].ToString()
                                        });
                                    }
                                }
                                //clientsFilter.ForEach(x =>
                                //{
                                //    x.Type = _context.Types.Where(y => y.CodeName == x.Type).FirstOrDefault().Description;
                                //});
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
                    if (agreement.AgreementDetails.Count > 0)
                    {
                        if (agreementvm.AgreementDetails.Count > 0)
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
                        if (agreementvm.ServicesId.Contains(x.IdService))
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

                    log.NewValue = JsonConvert.SerializeObject(agreement, new JsonSerializerSettings
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
            if (typeAgreement == null)
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
                        if (await _context.Agreements.Where(x => x.Account == agreementvm.Account).FirstOrDefaultAsync() != null)
                        {
                            return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "El número de cuenta ya fue asignado a otro contrato, Favor de verificar " });
                        }

                        if (agreementvm.AgreementPrincipalId != 0)
                        {
                            Principal = await _context.Agreements.Include(a => a.Addresses)
                                                                    .ThenInclude(s => s.Suburbs)
                                                                 .Where(x => x.Id == agreementvm.AgreementPrincipalId)
                                                                 .FirstOrDefaultAsync();
                            if (Principal.TypeAgreement == "AGR02")
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
                                if (item.TypeAddress == "DIR01")
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
                                IsActive = true,
                                TaxRegime = client.TaxRegime,

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
                            int a = await _context.SaveChangesAsync();
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



        [HttpPut("agreementDetail/{id}")]
        public async Task<IActionResult> PutAgreementDetail([FromRoute] int id, [FromBody]    AgreementDetail agree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    if (id != agree.AgreementId)
                    {
                        return BadRequest();
                    }

                    _context.Entry(agree).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AgreementListExist(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    scope.Complete();
                    return Ok(agree);

                }


            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(agree);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar detalle del contrato" });
            }



            return Ok(agree);
        }

        private bool AgreementListExist(int id)
        {
            return _context.AgreementDetails.Any(e => e.AgreementId == id);
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

            //if (agreement.TypeIntake.Acronym != "HA" || agreement.TypeIntake.Acronym != "UR")
            //{
            //    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
            //}

            if (agreement.TypeStateService.Id != 1 && agreement.TypeStateService.Id != 3)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });
            }

            if (agreement.AgreementDiscounts.Count >= 1)//suspendido
            {
                //if(agreement.AgreementDiscounts.)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El contrato no permite asignar más de un descuento, favor de verificar" });
            }

            if (discount.IsActive == false)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El descuento no se encuentra activo, favor de verificar" });
            }

            //if (discount.EndDate.Value < DateTime.UtcNow.ToLocalTime())
            //{
            //    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El descuento no se encuentra dentro de un periodo valido, favor de verificar" });
            //}


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
            return Ok("Descuento realizado");
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

            var valueSystem = await _context.SystemParameters.Where(x => x.Name == "ISMUNICIPAL").FirstOrDefaultAsync();


            if (valueSystem.TextColumn == "NO")
            {
                Agreement agreement = await _context.Agreements
                                               .Include(x => x.TypeIntake)
                                               .Include(x => x.TypeStateService)
                                               //.Include(x => x.AgreementDiscounts.Where(z => z.IsActive == true))
                                               .Where(x => x.Id == AgreementId)
                                               .FirstOrDefaultAsync();

                List<AgreementFile> file = await _context.AgreementFiles.Where(x => x.AgreementId == agreement.Id).ToListAsync();
                file.ForEach(x =>
                {
                    string name = AESEncryptionString.DecryptString(x.Name, appSettings.IssuerName);
                    int start = name.Length - 4;
                    x.Name = name.Remove(start, 4);
                });




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

                return Ok(new { discounts, file });
            }
            else
            {
                Agreement agreement = await _context.Agreements
                                             .Include(x => x.TypeIntake)
                                             .Include(x => x.TypeStateService)
                                             .Where(x => x.Id == AgreementId)
                                             .FirstOrDefaultAsync();

                List<AgreementFile> file = await _context.AgreementFiles.Where(x => x.AgreementId == agreement.Id).ToListAsync();

                file.ForEach(x =>
                {
                    string name = AESEncryptionString.DecryptString(x.Name, appSettings.IssuerName);
                    int start = name.Length - 4;
                    x.Name = name.Remove(start, 4);
                });

                //var catalogue = await _context.GroupCatalogues.Include(x => x.Catalogues).Where(x => x.Id == agreement.TypeUse.Id).FirstOrDefaultAsync();



                //if (catalogue != null)
                //{
                //    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Las características del contrato no permite el descuento, favor de verificar" });

                //}

                var discounts = await _context.AgreementDiscounts

                                             .Include(x => x.Discount)
                                             .Where(x => x.IdAgreement == AgreementId &&
                                                         x.IsActive == true).ToListAsync();

                return Ok(new { discounts, file });

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


        [HttpPost("sendMail/{sendMail}/{idAgreement}")]
        public async Task<IActionResult> SENDMAIL([FromRoute] int sendMail, int idAgreement)
        {
            if (ModelState.IsValid)
            {
                var value = _context.TaxReceipts.Where(x => x.PaymentId == sendMail).FirstOrDefault();

                if (value.XML == null || value.XML == " " || string.IsNullOrEmpty(value.XML))
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No cuenta con xml" });
                }

                var info = _context.Agreements.Where(a => a.Id == idAgreement).FirstOrDefault();

                var email = _context.Clients.Where(c => c.AgreementId == idAgreement).FirstOrDefault();

                if (email.EMail == null || email.EMail == " " || string.IsNullOrEmpty(email.EMail))
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No cuenta con email, favor de ingresar uno en la parte de contacto" });

                }

                var municipal = _context.SystemParameters.Where(a => a.Name == "ISMUNICIPAL").FirstOrDefault();

                if (municipal.TextColumn == "SI")
                {

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("cuautlancingo.gob.mx");
                    mail.From = new MailAddress("facturacion@cuautlancingo.gob.mx");
                    mail.To.Add(email.EMail);
                    //mail.To.Add("romero.urielj@gmail.com");
                    mail.Subject = "Envió de comprobante fiscal CFDI";
                    mail.AlternateViews.Add(getEmbeddedImage());
                    //mail.Body = string.Format(@"<!DOCTYPE html><html lang=""es""><head><meta charset=""UTF-8""><title>Facturación - Cuautlancingo</title><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/><link href=""https://fonts.googleapis.com/css?family=Montserrat|Roboto&display=swap"" rel=""stylesheet""></head><body style=""margin: 0; padding: 0;""><table style=""width: 100%; font-family: 'Roboto', sans-serif; letter-spacing: 0.50px;""><th style=""width: 7%;""><img src=""https://i.ibb.co/BGRJZGD/ayuntamiento-sistema.png"" alt=""Logo""></th><th style=""width: 93%;""><div style =""position: absolute !important; top: 26px; left:145px; color: #691A1B text-align: left;""><div><h2> Sistema de Facturación</h2></div><div style=""margin-bottom:25px;""><h5> Comprobante Fiscal CFDI</h5></div></div><div style=""background-color: #691A1B; width: 100%; height: 5px; position: relative; top: 35px;""></div></th><tr style=""font-family: 'Montserrat', sans-serif; ""><th colspan=""2"" style=""text-align: left; font-size: 15px; padding-left: 30px;""><br><br><p>Estimado Contribuyente:&nbsp;&nbsp; <strong></strong></p><p>Cuenta:&nbsp;&nbsp; <strong>{info.Account}</strong></p></p><br></th></tr><tr style=""font-family: 'Montserrat', sans-serif;""><th colspan=""2"" style=""text-align: justify; padding: 0px 80px 0px 80px;""><p> Usted está recibiendo un comprobante fiscal digital (FACTURA ELECTRÓNICA) del municipio de Cuautlancingo, Puebla 2018 - 2021. De acuerdo a la reglamentación del servicio de administración tributaria (SAT), publicada en el diario oficial de la federación (RMISC 2004)el 31 de Mayo del 2004, la factura electrónica es 100% valida y legal a partir de ahora, la entrega del documento fiscal (FACTURA ELECTRÓNICA)sera emitida y entregada por correo electrónico, cabe destacar que la factura electrónica se entregara en formato<small style=""color:#691A1B"">PDF</small> y archivo <small style=""color:#691A1B"">XML</small>, el cual podra imprimir libremente e incluirlaen su contabilidad (ARTICULO 29, FRACCION IV DE CFF), resguardar la impresión y archivo <small style=""color:#691A1B"">XML</small> por un periodode 5 años.<br><br><small style = ""font-size: 11px;""> Importante: contenido de la factura electrónica en el anexo 20 del diario oficial de la federación, publicadoel 1 de Septiembre de 2004, en el parrafo 2.22.8, se estipula que la impresión de la factura electrónica, que ademasde los datos fiscales y comerciales, deberá contener la cadena original, el certificado de sello digital, el sello digital y la leyenda: 'Este documento es una representación impresa de un CFD/CFDI'.</ small ></p></th></tr><tr style=""text-align: left; font-size: 15px; padding-left: 30px;""><th colspan=""2""><p>Observaciones</p><p></p></th></tr><tr><th colspan=""2""><br><br> <p style=""text-align: right; padding-right: 80px; font-size: 13px;""> **ESTE CORREO ES UNICAMENTE PARA ENVIOS, FAVOR DE NO RESPONDER</p> </th></tr><tr style=""background: url('https://i.ibb.co/2Kd9dfG/breadcrumb-bg1.jpg')""><th colspan=""2"" style=""width: 100%; height: 300px;""></th></tr></table></body></html>");

                    MemoryStream memoryStream = new MemoryStream();
                    System.Xml.XmlDocument xDocument = new System.Xml.XmlDocument();
                    xDocument.LoadXml(value.XML);
                    xDocument.Save(memoryStream);
                    memoryStream.Flush();//Adjust this if you want read your data 
                    memoryStream.Position = 0;
                    Stream stream = memoryStream;
                    Attachment attachment = new Attachment(stream, string.Format("Comprobante_" + info.Account + ".xml"), "application/xml");
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("facturacion@cuautlancingo.gob.mx", "e0P?k0k8");
                    SmtpServer.Send(mail);
                }
                else
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("sosapac.gob.mx");
                    mail.From = new MailAddress("facturacion@sosapac.gob.mx");
                    mail.To.Add(email.EMail);
                    //mail.To.Add("romero.urielj@gmail.com");
                    mail.Subject = "Envió de comprobante fiscal CFDI";
                    mail.AlternateViews.Add(getEmbeddedImage());
                    //mail.Body = string.Format(@"<!DOCTYPE html><html lang=""es""><head><meta charset=""UTF-8""><title>Facturación - Cuautlancingo</title><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/><link href=""https://fonts.googleapis.com/css?family=Montserrat|Roboto&display=swap"" rel=""stylesheet""></head><body style=""margin: 0; padding: 0;""><table style=""width: 100%; font-family: 'Roboto', sans-serif; letter-spacing: 0.50px;""><th style=""width: 7%;""><img src=""https://i.ibb.co/BGRJZGD/ayuntamiento-sistema.png"" alt=""Logo""></th><th style=""width: 93%;""><div style =""position: absolute !important; top: 26px; left:145px; color: #691A1B text-align: left;""><div><h2> Sistema de Facturación</h2></div><div style=""margin-bottom:25px;""><h5> Comprobante Fiscal CFDI</h5></div></div><div style=""background-color: #691A1B; width: 100%; height: 5px; position: relative; top: 35px;""></div></th><tr style=""font-family: 'Montserrat', sans-serif; ""><th colspan=""2"" style=""text-align: left; font-size: 15px; padding-left: 30px;""><br><br><p>Estimado Contribuyente:&nbsp;&nbsp; <strong></strong></p><p>Cuenta:&nbsp;&nbsp; <strong>{info.Account}</strong></p></p><br></th></tr><tr style=""font-family: 'Montserrat', sans-serif;""><th colspan=""2"" style=""text-align: justify; padding: 0px 80px 0px 80px;""><p> Usted está recibiendo un comprobante fiscal digital (FACTURA ELECTRÓNICA) del municipio de Cuautlancingo, Puebla 2018 - 2021. De acuerdo a la reglamentación del servicio de administración tributaria (SAT), publicada en el diario oficial de la federación (RMISC 2004)el 31 de Mayo del 2004, la factura electrónica es 100% valida y legal a partir de ahora, la entrega del documento fiscal (FACTURA ELECTRÓNICA)sera emitida y entregada por correo electrónico, cabe destacar que la factura electrónica se entregara en formato<small style=""color:#691A1B"">PDF</small> y archivo <small style=""color:#691A1B"">XML</small>, el cual podra imprimir libremente e incluirlaen su contabilidad (ARTICULO 29, FRACCION IV DE CFF), resguardar la impresión y archivo <small style=""color:#691A1B"">XML</small> por un periodode 5 años.<br><br><small style = ""font-size: 11px;""> Importante: contenido de la factura electrónica en el anexo 20 del diario oficial de la federación, publicadoel 1 de Septiembre de 2004, en el parrafo 2.22.8, se estipula que la impresión de la factura electrónica, que ademasde los datos fiscales y comerciales, deberá contener la cadena original, el certificado de sello digital, el sello digital y la leyenda: 'Este documento es una representación impresa de un CFD/CFDI'.</ small ></p></th></tr><tr style=""text-align: left; font-size: 15px; padding-left: 30px;""><th colspan=""2""><p>Observaciones</p><p></p></th></tr><tr><th colspan=""2""><br><br> <p style=""text-align: right; padding-right: 80px; font-size: 13px;""> **ESTE CORREO ES UNICAMENTE PARA ENVIOS, FAVOR DE NO RESPONDER</p> </th></tr><tr style=""background: url('https://i.ibb.co/2Kd9dfG/breadcrumb-bg1.jpg')""><th colspan=""2"" style=""width: 100%; height: 300px;""></th></tr></table></body></html>");

                    MemoryStream memoryStream = new MemoryStream();
                    System.Xml.XmlDocument xDocument = new System.Xml.XmlDocument();
                    xDocument.LoadXml(value.XML);
                    xDocument.Save(memoryStream);
                    memoryStream.Flush();//Adjust this if you want read your data 
                    memoryStream.Position = 0;
                    Stream stream = memoryStream;
                    Attachment attachment = new Attachment(stream, string.Format("Comprobante_" + info.Account + ".xml"), "application/xml");
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("facturacion@sosapac.gob.mx", "e0P?k0k8");
                    SmtpServer.Send(mail);
                }
            }
            return Ok("envio correcto");
        }

        private AlternateView getEmbeddedImage()
        {
            string htmlBody = string.Format(@"<!DOCTYPE html>
                                             <html lang=""es"">
                                             <head>
                                                <meta charset=""UTF-8"">
                                                 <title>Facturación - Cuautlancingo</title>
                                                 <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
                                                 <link href=""https://fonts.googleapis.com/css?family=Montserrat|Roboto&display=swap"" rel=""stylesheet"">
                                             </head>
                                             <body style=""margin: 0; padding: 0;"">
                                                 <table style=""width: 100%; font-family: 'Roboto', sans-serif; letter-spacing: 0.50px;"">
                                                     <th style=""width: 7%;""><img src=""https://i.ibb.co/BGRJZGD/ayuntamiento-sistema.png"" alt=""Logo""></th>
                                                     <th style=""width: 93%;"">
                                                        <div style =""position: absolute !important; top: 26px; left:145px; color: #691A1B text-align: left;"">
                                                            <div><h2> Sistema de Facturación</h2></div>
                                                            <div style=""margin-bottom:25px;""><h5> Comprobante Fiscal CFDI</h5></div>
                                                        </div>
                                                        <div style=""background-color: #691A1B; width: 100%; height: 5px; position: relative; top: 35px;""></div>
                                                     </th>
                                                     <tr style=""font-family: 'Montserrat', sans-serif; "">
                                                        <th colspan=""2"" style=""text-align: left; font-size: 15px; padding-left: 30px;"">
                                                            <br><br>
                                                            <p>Estimado Contribuyente:&nbsp;&nbsp; <strong></strong></p>
                                                            <p>Cuenta:&nbsp;&nbsp; <strong></strong></p></p>
                                                            <br>
                                                        </th>
                                                     </tr>
                                                     <tr style=""font-family: 'Montserrat', sans-serif;"">
                                                         <th colspan=""2"" style=""text-align: justify; padding: 0px 80px 0px 80px;"">
                                                             <p> Usted está recibiendo un comprobante fiscal digital (FACTURA ELECTRÓNICA) del municipio de Cuautlancingo, Puebla 2018 - 2021. 
                                                                 De acuerdo a la reglamentación del servicio de administración tributaria (SAT), publicada en el diario oficial de la federación (RMISC 2004)
                                                                 el 31 de Mayo del 2004, la factura electrónica es 100% valida y legal a partir de ahora, la entrega del documento fiscal (FACTURA ELECTRÓNICA)
                                                                 sera emitida y entregada por correo electrónico, cabe destacar que la factura electrónica se entregara en formato
                                                                 <small style=""color:#691A1B"">PDF</small> y archivo <small style=""color:#691A1B"">XML</small>, el cual podra imprimir libremente e incluirla
                                                                 en su contabilidad (ARTICULO 29, FRACCION IV DE CFF), resguardar la impresión y archivo <small style=""color:#691A1B"">XML</small> por un periodo
                                                                 de 5 años.
                                                                 <br><br>
                                                                 <small style = ""font-size: 11px;""> Importante: contenido de la factura electrónica en el anexo 20 del diario oficial de la federación, publicado
                                                                                                    el 1 de Septiembre de 2004, en el parrafo 2.22.8, se estipula que la impresión de la factura electrónica, que ademas
                                                                                                    de los datos fiscales y comerciales, deberá contener la cadena original, el certificado de sello digital, el sello
                                                                                                    digital y la leyenda: 'Este documento es una representación impresa de un CFD/CFDI'.</ small >
                                                            </p>
                                                         </th>
                                                     </tr>
                                                     <tr style=""text-align: left; font-size: 15px; padding-left: 30px;"">
                                                        <th colspan=""2"">
                                                            <p>Observaciones</p>
                                                            <p></p>
                                                        </th>
                                                    </tr>
                                                     <tr>
                                                         <th colspan=""2"">
                                                             <br><br>
                                                             <p style=""text-align: right; padding-right: 80px; font-size: 13px;""> **ESTE CORREO ES UNICAMENTE PARA ENVIOS, FAVOR DE NO RESPONDER</p>
                                                         </th>
                                                     </tr>
                                                     <tr style=""background: url('https://i.ibb.co/2Kd9dfG/breadcrumb-bg1.jpg')"">
                                                           <th colspan=""2"" style=""width: 100%; height: 300px;""></th>
                                                     </tr>
                                                   </table>
                                             </body>
                                             </html>");
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            return alternateView;
        }

        [HttpPost("addDebt/{idAgreement}/{dateAgreement}/{month}/{year}")]
        public async Task<IActionResult> PostDiscount([FromRoute] int idAgreement, int dateAgreement, int month, int year)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[billing_period]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
                    command.Parameters.Add(new SqlParameter("@date_agreement", dateAgreement));
                    command.Parameters.Add(new SqlParameter("@from_month", month));
                    command.Parameters.Add(new SqlParameter("@from_year", year));

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
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar la deuda: [{error}]") });

                    }
                    else
                    {
                        return Ok("Se genero la factura");

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


        [HttpPost("addDebtAyuntamiento/{idAgreement}/")]
        public async Task<IActionResult> PostDebtAyuntamiento([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[billing_period]";
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
                        return Ok("Se genero adeudo");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar la deuda: [{error}]") });


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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar adeudo" });
            }

        }




        [HttpPost("addDiscountAyuntamiento/{idAgreement}/")]
        public async Task<IActionResult> AddDiscountAgreement([FromRoute] int idAgreement)
        {
            var sDate = DateTime.Now.ToString();
            var datevalue = (Convert.ToDateTime(sDate.ToString()));

            //DateTime dy = datevalue.Day.ToString();
            var mn = datevalue.Month.ToString();
            var yy = datevalue.Year.ToString();

            int year = 0;

            int month = 0;

            year = Convert.ToInt32(yy);
            string error = string.Empty;
            try
            {
                var debt = await _context.Debts.Where(x => x.AgreementId == idAgreement && x.ExpirationDate.Year == year && x.Status == "ED001" && x.Type != "TIP02" && x.Type != "TIP03" && x.Type != "TIP05").ToListAsync();

                if (debt.Count == 0)
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se encontro deuda por pagar en este año") });

                }
                var discount = _context.AgreementDiscounts.Where(sd => sd.IdAgreement == idAgreement).FirstOrDefault();

                if (discount == null)
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El contrato no tiene agregado algun descuento") });
                }

                var valueD = _context.Discounts.Where(d => d.Id == discount.IdDiscount).FirstOrDefault();

                if (valueD == null)
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se encontraron descuentos") });

                }


                foreach (var one in debt)
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "[dbo].[billing_Adjustment]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id", one.AgreementId));
                        command.Parameters.Add(new SqlParameter("@porcentage_value", valueD.Percentage));
                        command.Parameters.Add(new SqlParameter("@discount_value", 0));
                        command.Parameters.Add(new SqlParameter("@text_discount", valueD.Name));
                        command.Parameters.Add(new SqlParameter("@option", 1));
                        command.Parameters.Add(new SqlParameter("@account_folio", 0));
                        //command.Parameters.Add(new SqlParameter("@error", 0));


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
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar el descuento: [{error}]") });

                        }

                    }
                }




                return Ok("Se genero correctamente el descuento");




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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar descuento" });
            }

        }

        [HttpPost("addDebtSosapac/{idAgreement}/{Month}/{Year}/{status}")]
        public async Task<IActionResult> PostDebtsSosapac([FromRoute] int idAgreement, int Month, int Year, string status)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[accrual_period_now]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
                    command.Parameters.Add(new SqlParameter("@from_month", Month));
                    command.Parameters.Add(new SqlParameter("@from_year", Year));
                    command.Parameters.Add(new SqlParameter("@status", status));
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
                        return Ok("Se genero adeudo");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar la deuda:{error}") });


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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar adeudo" });
            }

        }



        [HttpPost("addDebtSosapacBody")]
        public async Task<IActionResult> PostDebtsSosapacBody([FromBody] DebtSosapac sosapac)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[accrual_period_now]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", sosapac.idAgreement));
                    command.Parameters.Add(new SqlParameter("@from_month", sosapac.month));
                    command.Parameters.Add(new SqlParameter("@from_year", sosapac.year));
                    command.Parameters.Add(new SqlParameter("@status", sosapac.status));
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
                        return Ok("Se genero adeudo");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar la deuda:{error}") });


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
                systemLog.Parameter = sosapac.idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar adeudo" });
            }

        }

        [HttpPost("addDiscountDebt/{idAgreement}")]
        public async Task<IActionResult> PostDiscountDebt([FromRoute] int idAgreement)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[vulnerable_discount]";
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
                        return Ok("Se aplico el descuento");

                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo agregar el descuento: [{error}]") });


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
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el descuento" });
            }

        }

        [HttpGet("countAgrements/{year}/{mes}")]
        public async Task<IActionResult> CuntAgreements([FromRoute] string year, string mes)
        {
            var a = _context.Agreements.ToList();
            List<object> oc = new List<object>();
            a.ForEach(x =>
            {
                var paymenhts = _context.Payments.Include(p => p.PaymentDetails)
                .ThenInclude(pd => pd.Debt)
                .ToList();
                oc.Add(new { Agrement = x, Payments = paymenhts });
            });

            return null;
        }

        [HttpPost("getAccountStatus/{ISayuntamiento}")]
        public async Task<IActionResult> getAccountStatus([FromBody] List<string> id_agremmnents, [FromRoute] bool ISayuntamiento)
        {

            List<string> statusDeuda = new List<string>() { "ED001", "ED004", "ED007", "ED011" };
            System.Linq.IQueryable<Agreement> query = _context.Agreements.
                 Include(x => x.TypeService)
                   .Include(x => x.TypeUse)
                   .Include(x => x.AgreementDetails)
                   .Include(x => x.TypeIntake);
            if (!ISayuntamiento)
            {

                query = query
              .Include(x => x.TypeStateService);

            }

            //.Include(x => x.TypeStateService)
            //.Include(x => x.TypeRegime)
            //.Include(x => x.Diameter)

            var agreement = query.Include(x => x.Clients)
                   .Include(x => x.Addresses)
                       .ThenInclude(x => x.Suburbs)
                           .ThenInclude(x => x.Towns)
                               .ThenInclude(x => x.States)
               .Include(x => x.Debts)
                   .ThenInclude(x => x.DebtDetails)
               .Where(x => id_agremmnents.Contains(x.Id.ToString()))


               .ToList();


            agreement.ForEach(x =>
            {
                x.Debts = x.Debts.Where(d => statusDeuda.Contains(d.Status)).ToList();
                x.Clients = x.Clients.Where(c => c.TypeUser == "CLI01").ToList();
                x.Addresses = x.Addresses.Where(a => a.TypeAddress == "DIR01").ToList();

            });


            return Ok(agreement);
        }

        [HttpPost("GeneratePagosAnuales/{AgreementId}/{porcentajeDiscount}")]
        public async Task<IActionResult> GeneratePagosAnuales([FromRoute] int AgreementId, [FromRoute] int porcentajeDiscount, [FromBody] List<int> DebtsId)
        {
            //using (var transaction = _context.Database.BeginTransaction())
            //{
                DateTime date = DateTime.Now;
                try
                {
                foreach (var x in DebtsId) { 
                    //DebtsId.ForEach(x =>
                    
                        var PagoAnual = new PagosAnuales()
                        {
                            AgreementId = AgreementId,
                            DateDebt = date,
                            DebtId = x,
                            Status = "ED001"

                        };

                        _context.PagosAnuales.Add(PagoAnual);
                        await _context.SaveChangesAsync();
                        await ApplyDiscount(x, porcentajeDiscount);

                    }
                    //transaction.Commit();
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                return Ok();
            //}
            
        }

        private async Task ApplyDiscount(int DebtId, int porcentajeDiscount)
        {
            List<SPParameters> parameters = new List<SPParameters> {
                new SPParameters{Key ="id", Value = DebtId.ToString() },
                new SPParameters{Key ="porcentage_value", Value = porcentajeDiscount.ToString() },
                new SPParameters{Key ="discount_value", Value = "0" },
                new SPParameters{Key ="text_discount", Value = "Descuento aplicado por pago anual", DbType= DbType.String, Size = 50 },
                new SPParameters{Key ="option", Value = "1" },
                new SPParameters{Key ="account_folio", Value = "", Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Size = 30 },
                new SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}


                };
            var ss = await new RunSP(this, _context).runProcedureNT("billing_Adjusment", parameters);
        }

        [HttpPost("getSimulateDebt/{AgreementId}/{year}")]
        public async Task<IActionResult> getSimulateDebt([FromRoute] int AgreementId, [FromRoute] int year)
        {
            var result = _context.DebtAnnual.Where(x => x.AgreementId == AgreementId && x.Year == year).ToList();
           

            return Ok(result);

        }

    }






    public class CodeTraslator
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }




}