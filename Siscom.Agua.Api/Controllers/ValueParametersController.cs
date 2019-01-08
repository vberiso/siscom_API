using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ValueParametersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ValueParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(string value)
        {
            var today = DateTime.UtcNow.ToLocalTime().Date;
            var b = value.ToUpper();
            var val = _context.SystemParameters.Where(x => x.Name == b && x.IsActive == true && (x.StartDate <= today && x.EndDate >= today)).FirstOrDefault();
            return Ok(val);
        }

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
                                         .Where(x => x.MacAdress == mac &&
                                                     x.IsActive == true).FirstOrDefaultAsync();
            }

            if (terminal != null)
            {
                terminalUser = await _context.TerminalUsers
                                             .Where(x => x.Terminal == terminal &&
                                                         x.InOperation == true).FirstOrDefaultAsync();
            }
            else
                return NotFound();

            return Ok(new { terminal = terminal, terminalUser = terminalUser });
        }

        [HttpGet("Terminal/{teminalUserId}")]
        public async Task<IActionResult> GetTransactionCashBox([FromRoute] int teminalUserId)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            KeyValuePair<int, string> _typeTransaction = new KeyValuePair<int, string>(0, String.Empty);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Where(x => x.Id == teminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
            {
                return NotFound();
            }


            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                            x.TerminalUser.InOperation == true )
                                                .OrderBy(x => x.Id).ToListAsync();

            foreach (var item in movimientosCaja)
            {
                switch (item.TypeTransaction.Id)
                {
                    case 1://apertura
                        _typeTransaction = new KeyValuePair<int, string>(item.TypeTransaction.Id, item.TypeTransaction.Name);
                        break;
                    case 2://Fondo
                        _typeTransaction = new KeyValuePair<int, string>(item.TypeTransaction.Id, item.TypeTransaction.Name);
                        break;
                    case 5://Cierre
                        _typeTransaction = new KeyValuePair<int, string>(item.TypeTransaction.Id, item.TypeTransaction.Name);
                        break;
                    case 7: //Liquidada
                        _typeTransaction = new KeyValuePair<int, string>(item.TypeTransaction.Id, item.TypeTransaction.Name);
                        break;
                }
            }

            return Ok(_typeTransaction);
        }
    }
}