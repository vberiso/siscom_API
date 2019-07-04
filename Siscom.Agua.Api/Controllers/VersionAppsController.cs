using System;
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
    public class VersionAppsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VersionAppsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VersionApps
        [HttpGet]
        public IEnumerable<VersionApp> GetVersionApps()
        {
            return _context.VersionApps.Where(x => x.IsActive);
        }

        // GET: api/VersionApps/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVersionApp([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var versionApp = await _context.VersionApps.FindAsync(id);

            if (versionApp == null)
            {
                return NotFound();
            }

            return Ok(versionApp);
        }

        // PUT: api/VersionApps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVersionApp([FromRoute] int id, [FromBody] VersionApp versionApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != versionApp.Id)
            {
                return BadRequest();
            }

            _context.Entry(versionApp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VersionAppExists(id))
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

        // POST: api/VersionApps
        [HttpPost]
        public async Task<IActionResult> PostVersionApp([FromBody] VersionApp versionApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.VersionApps.Add(versionApp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVersionApp", new { id = versionApp.Id }, versionApp);
        }

        // DELETE: api/VersionApps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVersionApp([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var versionApp = await _context.VersionApps.FindAsync(id);
            if (versionApp == null)
            {
                return NotFound();
            }

            _context.VersionApps.Remove(versionApp);
            await _context.SaveChangesAsync();

            return Ok(versionApp);
        }

        private bool VersionAppExists(int id)
        {
            return _context.VersionApps.Any(e => e.Id == id);
        }
    }
}