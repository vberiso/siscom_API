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
        /// <param name="terminalUser">Model TerminalUser</param>
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

                       
            if (await _context.TerminalUsers.Where(x => x.TermianlId == pterminalUser.TermianlId &&
                                                   x.OpenDate.Date.Date == pterminalUser.OpenDate.Date.Date &&
                                                   x.InOperation==true)
                                            .FirstOrDefaultAsync() != null)
            {
                return StatusCode(409, new { Error = "La terminal ya se encuentra operando" });
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser.TermianlId = pterminalUser.TermianlId;
            terminalUser.UserId = pterminalUser.UserId;

            _context.TerminalUsers.Add(terminalUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTerminalUser", new { id = terminalUser.TermianlId }, terminalUser);
        }

        private bool Validate(TermimalUserVM pterminalUser)
        {
            if (pterminalUser.TermianlId == 0)
                return false;

            return true;
        }
    }
}
