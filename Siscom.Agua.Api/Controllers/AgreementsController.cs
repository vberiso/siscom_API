using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class AgreementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;

        public AgreementsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/Agreements
        [HttpGet]
        public IEnumerable<Agreement> GetAgreements()
        {
            return _context.Agreements.Include(a => a.Addresses)
                                    .Include(c => c.Clients);
        }


        // GET: api/Agreements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgreement([FromRoute] int id)
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
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
                                      .FirstOrDefaultAsync(a => a.Id == id);

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
                                      .Include(tss => tss.TypeStateService)
                                      .Include(ags => ags.AgreementServices)
                                        .ThenInclude(x => x.Service)
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

        // GET: api/Agreements
        [HttpGet("AgreementsBasic/{AcountNumber}")]
        public async Task<IActionResult> GetAgreementsBasic([FromRoute] string AcountNumber)
        {
            var agreement = _context.Agreements
                                    .Include(x => x.Clients)
                                    .Where(a => a.Account == AcountNumber).FirstOrDefault();

            if (agreement == null)
            {
                return NotFound();
            }
            return Ok(agreement);
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
            List<Agreement> agreement = new List<Agreement>();
            switch (search.Type)
            {
                case 1:
                    agreement.Add(await _context.Agreements.Include(a => a.Addresses)
                                                                .ThenInclude(s => s.Suburbs)
                                                           .Include(c => c.Clients)
                                      .FirstOrDefaultAsync(a => a.Account == search.StringSearch));
                    break;
                case 2:
                    var client = await _context.Clients.Include(x => x.Agreement)
                                                       .Where(x => x.ToString().Contains(search.StringSearch))
                                                       .ToListAsync();
                    foreach (var item in client)
                    {
                        agreement.Add(new Agreement
                        {
                            Account = item.Agreement.Account,
                            Id = item.Agreement.Id,
                            Clients = client,
                            Addresses = await _context.Adresses.Include(s => s.Suburbs).Where(x => x.AgreementsId == item.AgreementId).ToListAsync()
                        });
                    }
                    break;
                case 3:
                    var address = await _context.Adresses.Include(x => x.Agreements)
                                                         .Where(x => x.ToString().Contains(search.StringSearch))
                                                         .ToListAsync();
                    foreach (var item in address)
                    {
                        agreement.Add(new Agreement
                        {
                            Account = item.Agreements.Account,
                            Id = item.Agreements.Id,
                            Clients = await _context.Clients.Where(x => x.AgreementId == item.AgreementsId).ToListAsync(),
                            Addresses = address
                        });
                    }
                    break;
                case 4:
                    var clientrfc = await _context.Clients.Include(x => x.Agreement)
                                                        .Where(x => x.RFC.Contains(search.StringSearch))
                                                        .ToListAsync();
                    foreach (var item in clientrfc)
                    {
                        agreement.Add(new Agreement
                        {
                            Account = item.Agreement.Account,
                            Id = item.Agreement.Id,
                            Clients = clientrfc,
                            Addresses = await _context.Adresses.Include(s => s.Suburbs).Where(x => x.AgreementsId == item.AgreementId).ToListAsync()
                        });
                    }
                    break;
                default:
                    break;
            }


            if (agreement == null || agreement.Count == 0)
            {
                return NotFound();
            }

            return Ok(agreement);
        }

        // PUT: api/Agreements/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAgreement([FromRoute] int id, [FromBody] AgreementVM agreementvm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != agreement.Id)
        //    {
        //        return BadRequest();
        //    }

        //    //agreement
        //    _context.Entry(agreement).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AgreementExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}



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

            if (service != null && intake != null && use != null
                               && consume != null && regime != null
                               && cBusiness != null && sService != null
                               && period != null && diam != null
                               && agreementvm.ServicesId.Count > 0
                               && agreementvm.Adresses.Count > 0
                               && agreementvm.Clients.Count > 0)

            {

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        
                        if(agreementvm.AgreementPrincipalId != 0)
                        {
                            Principal = await _context.Agreements.Include(a => a.Addresses)
                                                                    .ThenInclude(s => s.Suburbs)
                                                                 .Where(x => x.Id == agreementvm.AgreementPrincipalId)
                                                                 .FirstOrDefaultAsync();
                        }

                        NewAgreement.Account = agreementvm.Account;
                        //NewAgreement.AccountDate = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                        NewAgreement.AccountDate = DateTime.UtcNow.ToLocalTime();
                        NewAgreement.NumDerivatives = agreementvm.Derivatives;
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


                        //
                        if (Principal != null)
                        {
                            agreementvm.Adresses.ForEach(async x => {
                                if(x.TypeAddress == "DIR01")
                                {
                                    var suburb = await _context.Suburbs.FindAsync(x.SuburbsId);
                                    if (Principal.Addresses.Where(p => p.TypeAddress == "DIR01").FirstOrDefault().Suburbs.Name != suburb.Name)
                                    {
                                        HasError = true;
                                    }
                                    else
                                    {
                                        Principal.Derivatives = Principal.Derivatives + 1;
                                        _context.Entry(Principal).State = EntityState.Modified;
                                        await _context.SaveChangesAsync();
                                        IsDerivative = true;
                                    }
                                }
                            });

                            if (HasError)
                            {
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El contrato no puede ser derivada, ya que no coincide la dirección o la colonia " });
                            }

                        }

                        foreach (var address in agreementvm.Adresses)
                        {
                            NewAgreement.Addresses.Add(new Adress
                            {
                                Street = address.Street,
                                Outdoor = address.Outdoor,
                                Indoor = address.Indoor,
                                Zip = address.Zip,
                                Reference = address.Reference,
                                Lat = address.Lat,
                                Lon = address.Lon,
                                TypeAddress = address.TypeAddress,
                                Suburbs = await _context.Suburbs.FindAsync(address.SuburbsId)
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
                                TypeUser = client.TypeUser
                            };

                            foreach (var item in client.Contacts)
                            {
                                nc.Contacts.Add(new Contact
                                {
                                    PhoneNumber = item.PhoneNumber,
                                    TypeNumber = item.TypeNumber
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
                                AgreementDerivative = NewAgreement.Id,
                                IsActive = true
                            };
                            await _context.Derivatives.AddAsync(derivative);
                            await _context.SaveChangesAsync();

                            AgreementLog agreementLogderivative = new AgreementLog()
                            {
                                Agreement = NewAgreement,
                                AgreementLogDate = DateTime.Now,
                                User = await userManager.FindByIdAsync(agreementvm.UserId),
                                Description = "Se Agrego Derivada al Contrato con Cuenta " + Principal.Account,
                                Observation = agreementvm.Observations
                            };
                            
                            await _context.AgreementLogs.AddAsync(agreementLogderivative);
                            int a  = await _context.SaveChangesAsync();
                        }
                        

                        foreach (var aservice in agreementvm.ServicesId)
                        {
                            await _context.AgreementServices.AddAsync(new AgreementService
                            {
                                Agreement = NewAgreement,
                                DateAgreement = DateTime.Now,
                                IdAgreement = NewAgreement.Id,
                                IdService = aservice,
                                IsActive = true,
                                Service = await _context.Services.FindAsync(aservice)
                            });
                            await _context.SaveChangesAsync();
                        }

                        AgreementLog agreementLog = new AgreementLog()
                        {
                            Agreement = NewAgreement,
                            AgreementLogDate = DateTime.Now,
                            User = await userManager.FindByIdAsync(agreementvm.UserId),
                            Description = "Nuevo Contrato",
                            Observation = agreementvm.Observations
                        };
                        await _context.AgreementLogs.AddAsync(agreementLog);


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

            return CreatedAtAction("GetAgreement", new { id = NewAgreement.Id }, NewAgreement);
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

        [HttpGet("GetData")]
        public async Task<IActionResult> GetInitialAgreements()
        {
            return Ok(new AgreementDataVM()
            {
                TypeService = await _context.TypeServices.ToListAsync(),
                TypeIntake = await _context.TypeIntakes.ToListAsync(),
                TypeUse = await _context.TypeUses.ToListAsync(),
                TypeConsume = await _context.TypeConsumes.ToListAsync(),
                TypeRegime = await _context.TypeRegimes.ToListAsync(),
                TypeCommertialBusiness = await _context.TypeCommertialBusinesses.ToListAsync(),
                TypeStateService = await _context.TypeStateServices.ToListAsync(),
                TypePeriod = await _context.TypePeriods.ToListAsync(),
                Diameter = await _context.Diameters.ToListAsync(),
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

                Services = await _context.Services
                                        .Where(s => s.InAgreement == true && s.IsActive == true)
                                        .Select(s => new ServiceVM
                                        {
                                            Id = s.Id,
                                            Name = s.Name
                                        }).ToListAsync()
            });
        }

        private bool AgreementExists(int id)
        {
            return _context.Agreements.Any(e => e.Id == id);
        }
    }
}