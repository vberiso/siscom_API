using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
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
            return _context.AssignmentTickets;
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

            return NoContent();
        }

        // POST: api/AssignmentTickets
        [HttpPost("{Initial}/{Final}")]
        public async Task<IActionResult> PostAssignmentTicket([FromRoute] int Initial ,int Final ,[FromBody] AssignmentTicket assignmentTicket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_context.AssignmentTickets
                       .Where(x=> x.Serie== assignmentTicket.Serie && x.Serie== assignmentTicket.Serie).FirstOrDefault() != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "No hay datos para este número de cuenta" });



            for ( int i = Initial; i < Final; i++ )
            {
                AssignmentTicket assignment = new AssignmentTicket();
                assignment.AssignmentDate = assignmentTicket.AssignmentDate;
                assignment.Folio = i;
                assignment.Status = assignmentTicket.Status;
                assignment.UserId = assignmentTicket.UserId;

                _context.AssignmentTickets.Add(assignmentTicket);
                await _context.SaveChangesAsync();
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