using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points Terminal
    /// </summary>
    [Route("api/Terminal")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TerminalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TerminalController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Terminal
        /// </summary>
        /// <returns></returns>
        // GET: api/Terminal
        [HttpGet]
        public IEnumerable<Terminal> GetTerminal()
        {
            return _context.Terminal;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Terminal which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>Terminal Model</returns>
        // GET: api/Terminal/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTerminal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var terminal = await _context.Terminal.FindAsync(id);

            if (terminal == null)
            {
                return NotFound();
            }

            return Ok(terminal);
        }

        /// <summary>
        /// This will provide update for the specific Terminal,
        /// </summary>
        /// <param name="pterminal">TerminalVM Model</param>
        /// <returns></returns>
        // PUT: api/Terminal/
        [HttpPut]
        public async Task<IActionResult> PutTerminal([FromBody] TerminalVM pterminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pterminal))
            {
                return StatusCode((int)TypeError.Code.NoContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            Terminal terminal = new Terminal();
            terminal.Id = pterminal.Id;
            terminal.MacAdress = pterminal.MacAdress;
            terminal.IsActive = pterminal.IsActive;
            terminal.CashBox = pterminal.CashBox;
            terminal.BranchOffice = await _context.BranchOffices.FindAsync(pterminal.BranchOffice);

            _context.Entry(terminal).State = EntityState.Modified;

            try
            {
                if (await _context.Terminal.Where(x => x.MacAdress == terminal.MacAdress &&
                                                  x.BranchOffice == terminal.BranchOffice &&
                                                  x.Id != terminal.Id)
                                           .FirstOrDefaultAsync() != null)
                    await _context.SaveChangesAsync();
                else
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La Mac Adres ya ha sido dado de alta para esta sucursal") });

            }
            catch (DbUpdateConcurrencyException)
            {
                if (terminal.BranchOffice == null)
                {
                    return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Sin Sucursal seleccionada") });
                }
                if (!TerminalExists(terminal.Id))
                {
                    return NotFound();
                }               
                throw;                
            }
            return NoContent();
        }



        /// <summary>
        /// This will provide capability add new Terminal
        /// </summary>
        /// <param name="pterminal">Model TerminalVM</param>
        /// <returns>New Terminal added</returns>
        // POST: api/Terminal
        [HttpPost]
        public async Task<IActionResult> PostTerminal([FromBody] TerminalVM pterminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pterminal.BranchOffice==0)
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            if (!MacAdressExists(pterminal.MacAdress))
            {
                Terminal terminal = new Terminal();
                terminal.MacAdress = pterminal.MacAdress;
                terminal.IsActive = pterminal.IsActive;
                terminal.CashBox = pterminal.CashBox;
                terminal.BranchOffice = await _context.BranchOffices.FindAsync(pterminal.BranchOffice);

                _context.Terminal.Add(terminal);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTerminal", new { id = terminal.Id }, terminal);
            }
            else
              return StatusCode((int)TypeError.Code.Conflict, new { Error = "La Mac Adress ya existe" });
        }

        /// <summary>
        /// This will provide delete for especific ID, of Terminal whitch is begin passed 
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns></returns>
        // DELETE: api/Terminal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerminal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var terminal = await _context.Terminal.FindAsync(id);
            if (terminal == null)
            {
                return NotFound();
            }

            _context.Terminal.Remove(terminal);
            await _context.SaveChangesAsync();

            return Ok(terminal);
        }

        private bool TerminalExists(int id)
        {
            return _context.Terminal.Any(e => e.Id == id);
        }

        private bool MacAdressExists(string macAdress)
        {
            return _context.Terminal.Any(e => e.MacAdress == macAdress);
        }

        private bool Validate(TerminalVM pterminal)
        {
            if (pterminal.Id == 0)
                return false;

            if (pterminal.BranchOffice == 0)
                return false;

            return true;
        }

    }
}
