using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points TerminalUser
    /// </summary>
    [Route("api/TerminalUser")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TerminalUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TerminalUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Terminal User
        /// </summary>
        /// <returns></returns>
        // GET: api/TerminalUser
        [HttpGet]
        public IEnumerable<TerminalUser> GetTerminalUser()
        {
            return _context.TerminalUsers;
        }

        /// <summary>
        /// This will provide capability add new TerminalUser
        /// </summary>
        /// <param name="pterminalUser">Model TerminalUserVM</param>
        /// <returns>New TerminalUser added</returns>
        // POST: api/TerminalUser
        [HttpPost]
        public async Task<IActionResult> PostTerminalUser([FromBody] TermimalUserVM pterminalUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pterminalUser))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            if(pterminalUser.OpenDate != DateTime.Now.Date)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Fecha incorrecta" });


            if (await _context.TerminalUsers.Where(x => x.Id == pterminalUser.TermianlId &&
                                                   x.OpenDate == pterminalUser.OpenDate &&
                                                   x.InOperation==true)
                                            .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "La terminal ya se encuentra operando" });
            }

            if (await _context.TerminalUsers.Where(x => x.User.Id == pterminalUser.UserId &&
                                                   x.InOperation == true)
                                            .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El usuario se encuentra activo en otra terminal" });
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser.Terminal = await _context.Terminal.FindAsync(pterminalUser.TermianlId);
            terminalUser.User = await _context.Users.FindAsync(pterminalUser.UserId);
            terminalUser.OpenDate = pterminalUser.OpenDate;
            terminalUser.InOperation = pterminalUser.InOperation;

            _context.TerminalUsers.Add(terminalUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTerminalUser", new { id = terminalUser.Id}, terminalUser);
        }

        /// <summary>
        /// This will provide update for the specific TerminalUser,
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <param name="pterminalUser">TerminalUserVM Model</param>
        /// <returns></returns>
        // PUT: api/TerminalUser/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerminalUser([FromRoute] int id, [FromBody] TermimalUserVM pterminalUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pterminalUser.Id)
            {
                return BadRequest();
            }

            if (!Validate(pterminalUser))
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            if (pterminalUser.OpenDate != DateTime.Now.Date)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Fecha incorrecta" });

            TerminalUser terminalUser = new TerminalUser();
            terminalUser.Id = pterminalUser.Id;
            terminalUser.Terminal = await _context.Terminal.FindAsync(pterminalUser.TermianlId);
            terminalUser.User = await _context.Users.FindAsync(pterminalUser.UserId);
            terminalUser.OpenDate = pterminalUser.OpenDate;
            terminalUser.InOperation = pterminalUser.InOperation;

            _context.Entry(terminalUser).State = EntityState.Modified;

            try
            {

                if (await _context.TerminalUsers.Where(x => x.Id == terminalUser.Id &&
                                                            x.OpenDate == terminalUser.OpenDate.Date &&
                                                            x.User.Id == terminalUser.User.Id &&
                                                            x.Terminal.Id == terminalUser.Terminal.Id)
                                           .FirstOrDefaultAsync() != null)
                {
                    await _context.SaveChangesAsync();                    
                }
                else
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se puede modificar una terminal en operación") });

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerminalUserExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return StatusCode((int)TypeError.Code.Ok, new { Error = string.Format("Modificación realizada con éxito") });
        }


        /// <summary>
        /// This will provide delete for especific ID, of TerminalUser whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/TerminalUser/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerminalUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var terminalUser = await _context.TerminalUsers.FindAsync(id);
            if (terminalUser == null)
            {
                return NotFound();
            }

            //validar transacciones

                _context.TerminalUsers.Remove(terminalUser);
            await _context.SaveChangesAsync();

            return Ok(terminalUser);
        }

        private bool Validate(TermimalUserVM pterminalUser)
        {
            if (pterminalUser.TermianlId == 0)
                return false;
            if (string.IsNullOrEmpty(pterminalUser.UserId))
                return false;
            if (string.IsNullOrEmpty(pterminalUser.OpenDate.ToString()))
                return false;

            return true;
        }

        private bool TerminalUserExists(int id)
        {
            return _context.TerminalUsers.Any(e => e.Id == id);
        }
    }
}
