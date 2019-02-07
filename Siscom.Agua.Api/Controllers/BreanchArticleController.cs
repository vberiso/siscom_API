using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
namespace Siscom.Agua.Api.Controllers
{
    [Route("api/BreachArticle/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class BreanchArticleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public BreanchArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BreanchArticle
        [HttpGet]
        public IEnumerable<BreachArticle> GetBreachArticles()
        {
            return _context.BreachArticles;
        }

        // GET: api/BreachArticle/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBreachArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breachArticle = await _context.BreachArticles.FindAsync(id);

            if (breachArticle == null)
            {
                return NotFound();
            }

            return Ok(breachArticle);
        }

        // POST: api/BreachArticle/1
        [HttpPost]
        public async Task<IActionResult> PostBreachArticle(int BreachArticleId, [FromBody] BreachArticle breachArticle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.BreachArticles.Add(breachArticle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBreachArticle", new { id = breachArticle.Id }, breachArticle);
        }


        // PUT: api/BreachArticle/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreachArticle([FromRoute] int id, [FromBody] BreachArticle breachArticle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != breachArticle.Id)
            {
                return BadRequest();
            }

            _context.Entry(breachArticle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreachArticleExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(breachArticle);
        }


        // DELETE: api/BreachArticle/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBreachArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breachArticle = await _context.BreachArticles.FindAsync(id);
            if (breachArticle == null)
            {
                return NotFound();
            }

            _context.BreachArticles.Remove(breachArticle);
            await _context.SaveChangesAsync();

            return Ok(breachArticle);
        }

        private bool BreachArticleExist(int id)
        {
            return _context.BreachArticles.Any(e => e.Id == id);
        }

    }
}
