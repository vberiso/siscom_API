using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    public class TransitPoliceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> UserManager;

        public TransitPoliceController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _context = context;
            UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        // GET: api/TransitPolice
        [HttpGet]
        public IEnumerable<TransitPolice> GetTransitPolices()
        {
            return _context.TransitPolices;
        }

        // GET: api/TransitPolice/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransitPolice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transitPolice = await _context.TransitPolices.FindAsync(id);

            if (transitPolice == null)
            {
                return NotFound();
            }

            return Ok(transitPolice);
        }

        // PUT: api/TransitPolice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransitPolice([FromRoute] int id, [FromBody] TransitPolice transitPolice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transitPolice.Id)
            {
                return BadRequest();
            }

            _context.Entry(transitPolice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransitPoliceExists(id))
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

        // POST: api/TransitPolice
        [HttpPost]
        public async Task<IActionResult> PostTransitPolice([FromBody] TransitPoliceVM transitPolice)
        {
            IdentityResult result;
            ApplicationUser user = new ApplicationUser()
            {
                Email = transitPolice.EMail,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = transitPolice.Name.Substring(0, 1) + transitPolice.LastName,
                Name = transitPolice.Name,
                LastName = transitPolice.LastName,
                SecondLastName = transitPolice.SecondLastName,
                DivitionId = 14,
                IsActive = true
            };
            string password = CrearPassword(6);
            result = await UserManager.CreateAsync(user, password);
           
                 if(result.Errors.Select(x => x.Code).Contains("DuplicateUserName"))
                {
                    int Fvalue = 0;
                    int.TryParse(user.UserName.Last().ToString(), out Fvalue);
                    user.UserName = Fvalue > 0 ? user.UserName.Remove(user.UserName.Count() - 1) + (Fvalue + 1) : user.UserName + 1;
                    result = await UserManager.CreateAsync(user, password);
                    if (result.Errors.Select(x => x.Code).Contains("DuplicateUserName"))
                    {
                        int value = 0; 
                        int.TryParse(user.UserName.Last().ToString(), out value);
                        user.UserName = user.UserName.Remove(user.UserName.Count() - 1) + (value + 1);
                        result = await UserManager.CreateAsync(user, password);
                    }}
            
            await UserManager.AddToRoleAsync(user, "Transito");

            if (result.Succeeded)
            {
                if (transitPolice.EMail == "N/A" || transitPolice.EMail == "n/a")
                {
                    TransitPolice police = new TransitPolice
                    {
                        Name = transitPolice.Name,
                        LastName = transitPolice.LastName,
                        SecondLastName = transitPolice.SecondLastName,
                        EMail = transitPolice.Name+transitPolice.LastName+"@gmail.com",
                        PhoneNumber = transitPolice.PhoneNumber,
                        Plate = transitPolice.Plate,
                        IsActive = true,
                        Address = transitPolice.Address,
                        User = user,
                        UserId = user.Id,
                    };


                    await _context.TransitPolices.AddAsync(police);
                    await _context.SaveChangesAsync();
                    return StatusCode((int)TypeError.Code.Ok, new { Error = $"Usuario creado con éxito {Environment.NewLine} Usuario: {user.UserName} {Environment.NewLine} Contraseña: {password} " });


                }
                else
                {
                    TransitPolice police = new TransitPolice
                    {
                        Name = transitPolice.Name,
                        LastName = transitPolice.LastName,
                        SecondLastName = transitPolice.SecondLastName,
                        EMail = transitPolice.EMail,
                        PhoneNumber = transitPolice.PhoneNumber,
                        Plate = transitPolice.Plate,
                        IsActive = true,
                        Address = transitPolice.Address,
                        User = user,
                        UserId = user.Id,
                    };

                    await _context.TransitPolices.AddAsync(police);
                    await _context.SaveChangesAsync();
                    return StatusCode((int)TypeError.Code.Ok, new { Error = $"Usuario creado con éxito {Environment.NewLine} Usuario: {user.UserName} {Environment.NewLine} Contraseña: {password} " });

                }


            }
            else
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });
            }
        }

        // DELETE: api/TransitPolice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransitPolice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transitPolice = await _context.TransitPolices.FindAsync(id);
            if (transitPolice == null)
            {
                return NotFound();
            }

            _context.TransitPolices.Remove(transitPolice);
            await _context.SaveChangesAsync();

            return Ok(transitPolice);
        }

        private bool TransitPoliceExists(int id)
        {
            return _context.TransitPolices.Any(e => e.Id == id);
        }

        private string CrearPassword(int longitud)
        {
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
    }
}