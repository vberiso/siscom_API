using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public IEnumerable<Client> GetClients([FromRoute] int AgreementId)
        {
            return _context.Clients.Include(c => c.Contacts).Where(x => x.AgreementId == AgreementId);
        }

        //// GET: api/Clients/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetClient([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var client = await _context.Clients.FindAsync(id);

        //    if (client == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(client);
        //}

        // PUT: api/Clients/5
        [HttpPut()]
        public async Task<IActionResult> PutClient([FromRoute] int AgreementId, [FromBody] UpdateClient cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var item in cliente.Client)
                    {
                        var clientBase = await _context.Clients.Include(c => c.Contacts).Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                        if(clientBase == null)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Los datos no coinciden con la información almacenada, Favor de verificar!" });

                        clientBase.CURP = item.CURP;
                        clientBase.EMail = item.EMail;
                        clientBase.INE = item.INE;
                        clientBase.IsActive = item.IsActive;
                        clientBase.LastName = item.LastName;
                        clientBase.Name = item.Name;
                        clientBase.RFC = item.RFC;
                        clientBase.SecondLastName = item.SecondLastName;
                        clientBase.TypeUser = item.TypeUser;
                        clientBase.Contacts.ToList().ForEach(x =>
                        {
                            var contact = item.Contacts.Where(i => i.Id == x.Id).FirstOrDefault();
                            x.IsActive = contact.IsActive;
                            x.PhoneNumber = contact.PhoneNumber;
                            x.TypeNumber = contact.TypeNumber;
                        });
                        _context.Entry(clientBase).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        item.Contacts.ToList().ForEach(x =>
                        {
                            if (!clientBase.Contacts.Any(z => z.PhoneNumber == x.PhoneNumber))
                            {
                                var contact = new Contact
                                {
                                    IsActive = 1,
                                    PhoneNumber = x.PhoneNumber,
                                    TypeNumber =  x.TypeNumber
                                };
                                clientBase.Contacts.Add(contact);
                                _context.SaveChanges();
                            }
                        });
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
                systemLog.Parameter = JsonConvert.SerializeObject(cliente);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar la dirección, Favor de verificar!" });
            }

            return NoContent();
        }

        // POST: api/Clients
        //[HttpPost]
        //public async Task<IActionResult> PostClient([FromBody] ClientVM client)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    Client NewClient = new Client()
        //    {
        //        Name = client.Name,
        //        LastName = client.LastName,
        //        SecondLastName = client.SecondLastName,
        //        RFC = client.RFC,
        //        CURP = client.CURP,
        //        INE = client.INE,
        //        EMail = client.EMail,
        //        TypeUser = client.TypeUser
        //    };
        //    _context.Clients.Add(NewClient);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetClient", new { id = client.Id }, client);
        //}

        //[HttpPost]
        //[Route("/api/Clients/AddContact")]
        //public async Task<IActionResult> PostClientContact([FromBody] ContactVM contact)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

          
        //    var client = _context.Clients.Find(contact.ClientId);
        //    if(client == null)
        //    {
        //        return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el Usuario") });
        //    }

        //    client.Contacts.Add(new Contact
        //    {
        //        PhoneNumber = contact.PhoneNumber,
        //        TypeNumber = contact.TypeNumber
        //    });
            
        //    //await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetClient", new { id = client.Id }, client);
        //}

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}