using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlinePaymentFilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OnlinePaymentFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OnlinePaymentFiles
        [HttpGet("All")]
        public IEnumerable<OnlinePaymentFile> GetOnlinePaymentFiles()
        {
            return _context.OnlinePaymentFiles;
        }

        // GET: api/OnlinePaymentFiles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOnlinePaymentFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var onlinePaymentFile = await _context.OnlinePaymentFiles.FindAsync(id);

            if (onlinePaymentFile == null)
            {
                return NotFound();
            }

            return Ok(onlinePaymentFile);
        }

        // PUT: api/OnlinePaymentFiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOnlinePaymentFile([FromRoute] int id, [FromBody] OnlinePaymentFile onlinePaymentFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != onlinePaymentFile.Id)
            {
                return BadRequest();
            }

            _context.Entry(onlinePaymentFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OnlinePaymentFileExists(id))
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

        // POST: api/OnlinePaymentFiles
        [HttpPost]
        public async Task<IActionResult> PostOnlinePaymentFile([FromBody] OnlinePaymentFile onlinePaymentFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OnlinePaymentFiles.Add(onlinePaymentFile);
            await _context.SaveChangesAsync();

            RedirectToActionResult redirect = new RedirectToActionResult("GetPdfById", "OnlinePaymentFiles", new { @id = onlinePaymentFile.Id });
            return redirect;
            //return CreatedAtAction("GetOnlinePaymentFile", new { id = onlinePaymentFile.Id }, onlinePaymentFile);
        }

        [HttpGet(Name = "GetPdfById")]
        public async Task<IActionResult> GetPdfById(int id)
        {
            var data = _context.Agreements.Find(id);
            return StatusCode((int)TypeError.Code.Ok, new { Success = "El número de cuenta asignado fue: " + data.Account });
        }

        // DELETE: api/OnlinePaymentFiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOnlinePaymentFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var onlinePaymentFile = await _context.OnlinePaymentFiles.FindAsync(id);
            if (onlinePaymentFile == null)
            {
                return NotFound();
            }

            _context.OnlinePaymentFiles.Remove(onlinePaymentFile);
            await _context.SaveChangesAsync();

            return Ok(onlinePaymentFile);
        }

        private bool OnlinePaymentFileExists(int id)
        {
            return _context.OnlinePaymentFiles.Any(e => e.Id == id);
        }
    }
}