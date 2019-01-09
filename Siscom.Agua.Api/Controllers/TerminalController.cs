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

            if (pterminal.BranchOffice == 0 || String.IsNullOrEmpty(pterminal.MacAdress))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            if (await _context.Terminal
                          .Where(x => x.MacAdress == pterminal.MacAdress &&
                                      x.IsActive == true)
                         .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La Terminal está activa en otra sucursal. Es necesario desactivarla para crear una nueva.") });

            Terminal terminal = new Terminal();
            terminal.MacAdress = pterminal.MacAdress;
            terminal.IsActive = pterminal.IsActive;
            terminal.CashBox = pterminal.CashBox;
            terminal.BranchOffice = await _context.BranchOffices.FindAsync(pterminal.BranchOffice);

            _context.Terminal.Add(terminal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTerminal", new { id = terminal.Id }, terminal);
        }

        /// <summary>
        /// Get the search result
        /// </summary>
        /// <param name="mac">MacAdress</param>
        /// /// <param name="branch">BranchOffice</param>
        /// <returns></returns>
        // GET: api/Terminal
        [HttpGet("{mac}/{branch}")]
        public async Task<IActionResult> FindTerminalUser([FromRoute] string mac, int branch)
        {
            string valores = String.Empty;
            var terminalUser = new object();
            Terminal terminal = new Terminal();

            if (!String.IsNullOrEmpty(mac) && branch != 0)
            {
                terminal = await _context.Terminal      
                                         .Include(x => x.BranchOffice)
                                         .Where(x => x.MacAdress == mac && x.BranchOffice.Id== branch).FirstOrDefaultAsync();
            }

            if (terminal == null)
            {
                return NotFound();
            }

            return Ok(terminal);
        }

        /// <summary>
        /// Get the search result
        /// </summary>
        /// <param name="mac">MacAdress</param>
        /// <returns></returns>
        // GET: api/Terminal
        [HttpGet("{mac}")]
        public async Task<IActionResult> FindTerminalMac([FromRoute] string mac)
        {
            string valores = String.Empty;
            Terminal terminal = new Terminal();
            TerminalUser terminalUser = new TerminalUser();

            if (!String.IsNullOrEmpty(mac))
            {
                terminal = await _context.Terminal
                                         .Include(x => x.BranchOffice)
                                         .Include(x => x.TerminalUsers)
                                         .Where(x => x.MacAdress == mac &&
                                                     x.IsActive == true).FirstOrDefaultAsync();
                terminal.TerminalUsers = terminal.TerminalUsers.Where(x => x.InOperation == true).ToList();
            }

            if (terminal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(terminal);
            }
        }

        private bool TerminalExists(int id)
        {
            return _context.Terminal.Any(e => e.Id == id);
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
