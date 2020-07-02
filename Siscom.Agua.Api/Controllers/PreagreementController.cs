using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreagreementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PreagreementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("FromFolio/{folio}")]
        public async Task<IActionResult> GetFromFolio([FromRoute] string folio)
        {
            try
            {
                List<PreAgreement> preAgreements = await _context.PreAgreements
                                                .Include(x => x.TypeIntake)
                                                .Include(x => x.TypeService)
                                                .Include(x => x.TypeUse)
                                                .Include(x => x.TypeClassification)
                                                .Include(x => x.Suburbs)
                                                .Where(p => p.Folio.Equals(folio)).ToListAsync();
                return Ok(preAgreements);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.NoContent , new { Error = string.Format("Problemas al tratar de consultar: {0}", ex.Message) });
            }
        }

        [HttpGet("FromUser/{name?}/{lastname?}/{secondlastname?}")]
        public async Task<IActionResult> GetFromUser([FromRoute] string name = "", [FromRoute] string lastname = "", [FromRoute] string secondlastname = "")
        {
            try
            {
                List<PreAgreement> preAgreements = await _context.PreAgreements
                                                .Include(x => x.TypeIntake)
                                                .Include(x => x.TypeService)
                                                .Include(x => x.TypeUse)
                                                .Include(x => x.TypeClassification)
                                                .Include(x => x.Suburbs)
                                                .Where(p => p.ClientName.Contains(name.Equals("-") ? "" : name) && p.ClientLastName.Contains(lastname.Equals("-")?"":lastname) && p.ClientSecondLastName.Contains(secondlastname.Equals("-")?"":secondlastname))
                                                .ToListAsync();
                return Ok(preAgreements);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("Problemas al tratar de consultar: {0}", ex.Message) });
            }
        }

        [HttpGet("FromAddress/{idSuburb}/{street}/{outdoor}")]
        public async Task<IActionResult> GetFromAddress([FromRoute] int idSuburb, [FromRoute] string street, [FromRoute] string outdoor)
        {
            try
            {
                List<PreAgreement> preAgreements = await _context.PreAgreements
                                                .Include(x => x.TypeIntake)
                                                .Include(x => x.TypeService)
                                                .Include(x => x.TypeUse)
                                                .Include(x => x.TypeClassification)
                                                .Include(x => x.Suburbs)
                                                .Where(p => p.SuburbsId == idSuburb && p.Street.Contains(street.Equals("-")?"":street) && p.Outdoor.Contains(outdoor.Equals("-")?"":outdoor))
                                                .ToListAsync();
                return Ok(preAgreements);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("Problemas al tratar de consultar: {0}", ex.Message) });
            }
        }

        [HttpGet("Suburbs")]
        public async Task<IActionResult> GetSuburbs()
        {
            try
            {
                var data = await _context.Suburbs.Where(s => s.TownsId == 2).ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("No se encontro informacion: {0}", ex.Message) });
            }
        }

        [HttpGet("Catalogos")]
        public async Task<IActionResult> GetCalalogos()
        {
            try
            {
                List<TypeIntake> typeIntakes = await _context.TypeIntakes.ToListAsync();
                List<TypeService> typeServices = await _context.TypeServices.ToListAsync();
                List<TypeUse> typeUses = await _context.TypeUses.ToListAsync();
                List<TypeClassification> typeClassifications = await _context.TypeClassifications.ToListAsync();
                List<Suburb> suburbs = await _context.Suburbs.Where(s => s.TownsId == 2).ToListAsync();
                List<Service> services = await _context.Services.Where(s => s.InAgreement == true).ToListAsync(); 
                return Ok(new List<object>() { typeIntakes, typeServices, typeUses, typeClassifications, suburbs, services });
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("No se encontro informacion: {0}", ex.Message) });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> postUpdate([FromBody] PreAgreement preAgreement)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Agreement agreement = new Agreement()
                    {
                        AccountDate = DateTime.Now,
                        //Derivatives = null,
                        StratDate = DateTime.Now,
                        TypeAgreement = "AGR01",
                        Route = "0",
                        TypeServiceId = preAgreement.TypeServiceId,
                        TypeUseId = preAgreement.TypeUseId,
                        TypeConsumeId = 2,
                        TypeRegimeId = 1,
                        TypePeriodId = 1,
                        TypeCommertialBusinessId = 1,
                        TypeStateServiceId = 1,
                        TypeIntakeId = preAgreement.TypeIntakeId,
                        DiameterId = 1,
                        TypeClassificationId = preAgreement.TypeClassificationId
                    };
                    _context.Agreements.Add(agreement);
                    _context.SaveChanges();

                    Client client = new Client()
                    {
                        Name = preAgreement.ClientName,
                        LastName = preAgreement.ClientLastName,
                        SecondLastName = preAgreement.ClientSecondLastName,
                        RFC = "XAXX010101000",
                        EMail = "",
                        IsActive = true,
                        TypeUser = "CLI01",
                        AgreementId = agreement.Id,
                        TaxRegime = false,
                        Contacts = null
                    };
                    _context.Clients.Add(client);
                    _context.SaveChanges();

                    Address address = new Address()
                    {
                        Street = preAgreement.Street,
                        Outdoor = preAgreement.Outdoor,
                        Indoor = preAgreement.Indoor,
                        Zip = preAgreement.Zip,
                        Reference = preAgreement.Reference,
                        Lat = preAgreement.Lat,
                        Lon = preAgreement.Lon,
                        TypeAddress = "DIR01",
                        IsActive = true,
                        AgreementsId = agreement.Id,
                        SuburbsId = preAgreement.SuburbsId
                    };
                    _context.Adresses.Add(address);
                    _context.SaveChanges();

                    if(preAgreement.ServiceId1 != 0)
                    {
                        AgreementService agreementService = new AgreementService() 
                        {
                            IdService = preAgreement.ServiceId1,
                            IdAgreement = agreement.Id,
                            DateAgreement = DateTime.Now,
                            IsActive = true                            
                        };
                        _context.AgreementServices.Add(agreementService);                        
                    }
                    if (preAgreement.ServiceId2 != 0)
                    {
                        AgreementService agreementService = new AgreementService()
                        {
                            IdService = preAgreement.ServiceId2,
                            IdAgreement = agreement.Id,
                            DateAgreement = DateTime.Now,
                            IsActive = true
                        };
                        _context.AgreementServices.Add(agreementService);
                    }
                    if (preAgreement.ServiceId3 != 0)
                    {
                        AgreementService agreementService = new AgreementService()
                        {
                            IdService = preAgreement.ServiceId3,
                            IdAgreement = agreement.Id,
                            DateAgreement = DateTime.Now,
                            IsActive = true
                        };
                        _context.AgreementServices.Add(agreementService);
                    }
                    _context.SaveChanges();

                    preAgreement.agreementId_new = agreement.Id;
                    _context.PreAgreements.Update(preAgreement);
                    _context.SaveChanges();


                    //se ejecuta el sp para el token de agreement
                    string error = string.Empty;
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "[dbo].[GenerateToken]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 6000;

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
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        if (!string.IsNullOrEmpty(error))
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "No se pudo generar el token de agreement" });
                        }
                    }

                    scope.Complete();
                }
                
                return Ok("Proceso exitoso");
            }
            catch(Exception ex)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = ex.Message;
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "PreagreementController";
                systemLog.Action = "postPreagreement";
                systemLog.Parameter = JsonConvert.SerializeObject(preAgreement);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para guardar un nuevo agreement." });
            }
        }

    }
}