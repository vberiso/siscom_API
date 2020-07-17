using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TestProgramacion : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestProgramacion(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<TestProgramacion>
        [HttpGet]
        public IEnumerable<TaxUserVM> Get()
        {
            var data = _context.TaxUsers
                .Include(x => x.TaxAddresses)
                .Select(x => new TaxUserVM
                {
                    Id = x.Id,
                    CURP = x.CURP,
                    EMail = x.EMail,
                    IsActive = x.IsActive,
                    IsProvider = x.IsProvider,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    RFC = x.RFC,
                    TaxAddresses = new List<TaxAddressVM> {
                        new TaxAddressVM() {
                            Id = x.TaxAddresses.FirstOrDefault().Id,
                            Indoor = x.TaxAddresses.FirstOrDefault().Indoor,
                            Outdoor = x.TaxAddresses.FirstOrDefault().Outdoor,
                            State = x.TaxAddresses.FirstOrDefault().State,
                            Street = x.TaxAddresses.FirstOrDefault().Street,
                            Suburb = x.TaxAddresses.FirstOrDefault().Suburb,
                            Town = x.TaxAddresses.FirstOrDefault().Town,
                            Zip = x.TaxAddresses.FirstOrDefault().Zip
                        }
                    }
                })
                .Take(20);
            return data;
        }

        // GET api/<TestProgramacion>/5
        [HttpGet("{id}")]
        public TaxUserVM Get(int id)
        {
            return _context.TaxUsers
                .Include(x => x.TaxAddresses)
                .Select(x => new TaxUserVM
                {
                    Id = x.Id,
                    CURP = x.CURP,
                    EMail = x.EMail,
                    IsActive = x.IsActive,
                    IsProvider = x.IsProvider,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    RFC = x.RFC,
                    TaxAddresses = new List<TaxAddressVM> {
                        new TaxAddressVM() {
                            Id = x.TaxAddresses.FirstOrDefault().Id,
                            Indoor = x.TaxAddresses.FirstOrDefault().Indoor,
                            Outdoor = x.TaxAddresses.FirstOrDefault().Outdoor,
                            State = x.TaxAddresses.FirstOrDefault().State,
                            Street = x.TaxAddresses.FirstOrDefault().Street,
                            Suburb = x.TaxAddresses.FirstOrDefault().Suburb,
                            Town = x.TaxAddresses.FirstOrDefault().Town,
                            Zip = x.TaxAddresses.FirstOrDefault().Zip
                        }
                    }
                }).Where(x => x.Id == id)
                .FirstOrDefault();
        }

        // POST api/<TestProgramacion>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaxUserVM taxUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TaxAddress address = new TaxAddress
            {
                Indoor = taxUser.TaxAddresses.FirstOrDefault().Indoor,
                Outdoor = taxUser.TaxAddresses.FirstOrDefault().Outdoor,
                State = taxUser.TaxAddresses.FirstOrDefault().State,
                Street = taxUser.TaxAddresses.FirstOrDefault().Street,
                Suburb = taxUser.TaxAddresses.FirstOrDefault().Suburb,
                Town = taxUser.TaxAddresses.FirstOrDefault().Town,
                Zip = taxUser.TaxAddresses.FirstOrDefault().Zip,
            };

            TaxUser user = new TaxUser
            {
                CURP = taxUser.CURP,
                EMail = taxUser.EMail,
                IsActive = taxUser.IsActive,
                IsProvider = taxUser.IsProvider,
                Name = taxUser.Name,
                PhoneNumber = taxUser.PhoneNumber,
                RFC = taxUser.RFC,
                TaxAddresses = new List<TaxAddress>() { address }
            };
            
            _context.TaxUsers.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }

        // PUT api/<TestProgramacion>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaxUserVM value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != value.Id)
            {
                return BadRequest();
            }

            TaxUser taxUser = await _context.TaxUsers.FindAsync(id);
            taxUser.CURP = value.CURP;
            taxUser.EMail = value.EMail;
            taxUser.IsActive = value.IsActive;
            taxUser.IsProvider = value.IsProvider;
            taxUser.Name = value.Name;
            taxUser.PhoneNumber = value.PhoneNumber;
            taxUser.RFC = value.RFC;

            TaxAddress taxAddress = await _context.TaxAddresses.FindAsync(value.TaxAddresses.FirstOrDefault().Id);
            taxAddress.Indoor = value.TaxAddresses.FirstOrDefault().Indoor;
            taxAddress.Outdoor = value.TaxAddresses.FirstOrDefault().Outdoor;
            taxAddress.State = value.TaxAddresses.FirstOrDefault().State;
            taxAddress.Street = value.TaxAddresses.FirstOrDefault().Street;
            taxAddress.Suburb = value.TaxAddresses.FirstOrDefault().Suburb;
            taxAddress.Town = value.TaxAddresses.FirstOrDefault().Town;
            taxAddress.Zip = value.TaxAddresses.FirstOrDefault().Zip;
            taxAddress.TaxUser = taxUser;
            taxAddress.TaxUserId = taxUser.Id;

            _context.Entry(taxAddress).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxUserExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();

        }

        // DELETE api/<TestProgramacion>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxUser = await _context.TaxUsers.FindAsync(id);
            if (taxUser == null)
            {
                return NotFound();
            }

            _context.TaxUsers.Remove(taxUser);
            await _context.SaveChangesAsync();

            return Ok(taxUser);
        }

        private bool TaxUserExist(int id)
        {
            return _context.TaxUsers.Any(e => e.Id == id);
        }
    }
}
