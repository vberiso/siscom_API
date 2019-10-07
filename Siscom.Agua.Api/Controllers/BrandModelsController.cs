using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
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
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class BrandModelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BrandModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BrandModels
        [HttpGet]
        public IEnumerable<BrandModel> GetBrandModels()
        {
            return _context.BrandModels;
        }

        // GET: api/BrandModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandModel([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brandModel = await _context.BrandModels.FindAsync(id);

            if (brandModel == null)
            {
                return NotFound();
            }

            return Ok(brandModel);
        }

        // PUT: api/BrandModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrandModel([FromRoute] int id, [FromBody] BrandModel brandModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != brandModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(brandModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandModelExists(id))
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

        // POST: api/BrandModels
        [HttpPost]
        public async Task<IActionResult> PostBrandModel([FromBody] BrandModel brandModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BrandModels.Add(brandModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBrandModel", new { id = brandModel.Id }, brandModel);
        }

        // DELETE: api/BrandModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrandModel([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brandModel = await _context.BrandModels.FindAsync(id);
            if (brandModel == null)
            {
                return NotFound();
            }

            _context.BrandModels.Remove(brandModel);
            await _context.SaveChangesAsync();

            return Ok(brandModel);
        }

        private bool BrandModelExists(int id)
        {
            return _context.BrandModels.Any(e => e.Id == id);
        }
    }
}