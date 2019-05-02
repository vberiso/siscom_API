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
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class AssignmentTicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssignmentTicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AssignmentTickets
        [HttpGet]
        public IEnumerable<AssignmentTicket> GetAssignmentTickets()
        {
            return _context.AssignmentTickets.Include(c=>c.TransitPolice);
        }

        // GET: api/AssignmentTickets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignmentTicket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmentTicket = await _context.AssignmentTickets.FindAsync(id);

            if (assignmentTicket == null)
            {
                return NotFound();
            }

            return Ok(assignmentTicket);
        }

        // PUT: api/AssignmentTickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignmentTicket([FromRoute] int id, [FromBody] AssignmentTicket assignmentTicket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != assignmentTicket.Id)
                    {
                        return BadRequest();
                    }

                    _context.Entry(assignmentTicket).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AssignmentTicketExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    scope.Complete();
                    return Ok(assignmentTicket);

                }
            }
            catch (Exception e)
            {

                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(assignmentTicket);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para modificar boletos" });


            }

           
        }

        // POST: api/AssignmentTickets
        [HttpPost("{Initial}/{Final}")]
        public async Task<IActionResult> PostAssignmentTicket([FromRoute] int Initial ,int Final ,[FromBody] AssignmentTicket assignmentTicket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var value = _context.AssignmentTickets.OrderBy(x => x.Folio).ToListAsync();


            if (Initial==0 || Final ==0)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Rango de folios incorrectos" });
           if(assignmentTicket.TransitPoliceId==0)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe proporcionar un agente" });

           if((await _context.TransitPolices.Where(x=> x.Id== assignmentTicket.TransitPoliceId).FirstOrDefaultAsync())== null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "No se ha localizado agente para asignar folios" });

            //Fata Validar que el rango de folios no esté dado de alta previamente
            //EFT03 CANCELADA; EFT02 ASIGNADO; EFT01= SIN ASIGNAR




            for ( int i = Initial; i <= Final; i++ )
            {
                var value = await _context.AssignmentTickets.Where(x => x.Folio == i).FirstOrDefaultAsync();
                if (value ==  null)
                {
                    AssignmentTicket assignment = new AssignmentTicket();
                    assignment.AssignmentDate = DateTime.UtcNow.ToLocalTime();
                    assignment.Folio = i;
                    assignment.Status = "EFT01";
                    assignment.TransitPoliceId = assignmentTicket.TransitPoliceId;

                    _context.AssignmentTickets.Add(assignment);
                    await _context.SaveChangesAsync();


                }
                else
                {
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El folio " + i + " existe, cambie su rango" });

                }




            }
          

            return CreatedAtAction("GetAssignmentTicket", new { id = assignmentTicket.Id }, assignmentTicket);
        }

        // DELETE: api/AssignmentTickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignmentTicket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmentTicket = await _context.AssignmentTickets.FindAsync(id);
            if (assignmentTicket == null)
            {
                return NotFound();
            }

            _context.AssignmentTickets.Remove(assignmentTicket);
            await _context.SaveChangesAsync();

            return Ok(assignmentTicket);
        }

        private bool AssignmentTicketExists(int id)
        {
            return _context.AssignmentTickets.Any(e => e.Id == id);
        }
    }
}