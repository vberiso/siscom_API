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
    [Route("api/Agreement/{AgreementId}/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class AdressesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Adresses
        [HttpGet]
        public async Task<IActionResult> GetAdresses([FromRoute] int AgreementId)
        {
            var address = await _context.Adresses
                .Include(s => s.Suburbs)
                .Where(i => i.AgreementsId == AgreementId).ToListAsync();
            address.ToList().ForEach(X =>
            {
                X.Suburbs = _context.Suburbs.Include(r => r.Regions)
                                            .Include(c => c.Clasifications)
                                            .Include(t => t.Towns)
                                                .ThenInclude(s => s.States)
                                                .ThenInclude(c => c.Countries)
                                            .Where(i => i.Id == X.Suburbs.Id)
                                            .SingleOrDefault();
            });
            if(address == null)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Los datos no coinciden con la información almacenada, Favor de verificar!" });

            return Ok(address);
        }

        // GET: api/Adresses/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetAdress([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var adress = await _context.Adresses.FindAsync(id);

        //    if (adress == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(adress);
        //}

        // PUT: api/Adresses/5
        [HttpPut()]
        public async Task<IActionResult> PutAdress([FromRoute] int AgreementId, [FromBody] UpdateAddress adressvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var item in adressvm.Addresses)
                    {
                        if(item.Id == 0)
                        {
                            Address a = new Address
                            {
                                AgreementsId = AgreementId,
                                Indoor = item.Indoor,
                                IsActive = item.IsActive,
                                Lat = item.Lat,
                                Lon = item.Lon,
                                Outdoor = item.Outdoor,
                                Reference = item.Reference,
                                Street = item.Street,
                                SuburbsId = item.SuburbsId,
                                TypeAddress = item.TypeAddress,
                                Zip = item.Zip,
                                Agreements = await _context.Agreements.FindAsync(AgreementId),
                                Suburbs = await _context.Suburbs.FindAsync(item.SuburbsId)
                            };
                            await _context.Adresses.AddAsync(a);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var add = _context.Adresses.Include(s => s.Suburbs).Where(i => i.Id == item.Id).FirstOrDefault();
                            if (add == null)
                                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Los datos no coinciden con la información almacenada, Favor de verificar!" });
                            add.Indoor = item.Indoor;
                            add.IsActive = item.IsActive;
                            add.Lat = item.Lat;
                            add.Lon = item.Lon;
                            add.Outdoor = item.Outdoor;
                            add.Reference = item.Reference;
                            add.Street = item.Street;
                            add.Suburbs = _context.Suburbs.Find(item.SuburbsId);
                            add.SuburbsId = item.SuburbsId;
                            add.TypeAddress = item.TypeAddress;
                            add.Zip = item.Zip;

                            _context.Entry(add).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                     
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
                systemLog.Parameter = JsonConvert.SerializeObject(adressvm);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar la dirección, Favor de verificar!" });
            }

            return NoContent();
        }


        [HttpPut("UpdateCoodinates")]
        public async Task<IActionResult> PutUpdateCoordinates([FromRoute] int idAgreement, [FromBody] string adressvm)
        {
            var Coordenadas = new { id = int.Parse(adressvm.Split(',')[0]) , Lat = adressvm.Split(',')[1], Lon = adressvm.Split(',')[2] };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {                
                var add = _context.Adresses.FirstOrDefault(i => i.Id == Coordenadas.id);
                if (add == null)
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Los datos no coinciden con la información almacenada, Favor de verificar!" });
                add.Lat = Coordenadas.Lat;
                add.Lon = Coordenadas.Lon;

                var Actualizado = _context.Attach(add);
                Actualizado.Property(x => x.Lat).IsModified = true;
                Actualizado.Property(x => x.Lon).IsModified = true;
                await _context.SaveChangesAsync();
                return Ok("Actualización exitosa");
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(adressvm);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar las coordenadas, Favor de verificar!" });
            }

            return NoContent();
        }


        [HttpGet("GetAddressByPostalCode/{postalCode}")]
        public async Task<List<Suburb>> GetMunicipioByPostalCode([FromRoute] string postalCode)
        {
            var data = _context.Suburbs.Where(x => x.Zip == postalCode).ToList();

            if (data.Count == 0)
            {
                return new List<Suburb>();
            }
            var Municipio = _context.Towns.Where(x => x.Id == data.First().TownsId).Select(x => new Town() { Id = x.Id, Name = x.Name, StateId = x.StateId }).First();
            var Estado = _context.States.Where(x => x.Id == Municipio.StateId).Select(x => new State() { Id = x.Id, Name = x.Name, CountriesId = x.CountriesId }).First();
            Municipio.States = Estado;
            var Pais = _context.Countries.Where(x => x.Id == Estado.CountriesId).First();
            Estado.Countries = Pais;
            var Colonias = new List<Suburb>();
            foreach (var colonia in data)
            {
                colonia.Towns = Municipio;
                Colonias.Add(colonia);
            }

            return JsonConvert.DeserializeObject<List<Suburb>>(JsonConvert.SerializeObject(Colonias, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           }));
        }

        // POST: api/Adresses
        //[HttpPost]
        //public async Task<IActionResult> PostAdress([FromBody] AdressVM adress)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Address NewAdress = new Address()
        //    {
        //        Street = adress.Street,
        //        Outdoor = adress.Outdoor,
        //        Indoor = adress.Indoor,
        //        Zip = adress.Zip,
        //        Reference = adress.Reference,
        //        Lat = adress.Lat,
        //        Lon = adress.Lon,
        //        TypeAddress = adress.TypeAddress
        //    };

        //    _context.Adresses.Add(NewAdress);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetAdress", new { id = adress.Id }, adress);
        //}

        // DELETE: api/Adresses/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAdress([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var adress = await _context.Adresses.FindAsync(id);
        //    if (adress == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Adresses.Remove(adress);
        //    await _context.SaveChangesAsync();

        //    return Ok(adress);
        //}

        private bool AdressExists(int id)
        {
            return _context.Adresses.Any(e => e.Id == id);
        }
    }
}