using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public IEnumerable<Client> GetClients()
        {
            return _context.Clients;
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient([FromRoute] int id)
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

            return Ok(client);
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient([FromRoute] int id, [FromBody] ClientVM client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clientBase = _context.Clients.Find(client.Id);
            if(clientBase == null)
            {

            }

            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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

        [HttpPost]
        [Route("/api/Clients/AddContact")]
        public async Task<IActionResult> PostClientContact([FromBody] ContactVM contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          
            var client = _context.Clients.Find(contact.ClientId);
            if(client == null)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = string.Format("Favor de verificar el Usuario") });
            }

            client.Contacts.Add(new Contact
            {
                PhoneNumber = contact.PhoneNumber,
                TypeNumber = contact.TypeNumber
            });
            
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

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