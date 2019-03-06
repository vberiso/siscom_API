﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpPost]
        public async Task<IActionResult> PostAssignmentTicket([FromBody] AssignmentTicket assignmentTicket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AssignmentTickets.Add(assignmentTicket);
            await _context.SaveChangesAsync();

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