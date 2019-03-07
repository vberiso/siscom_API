using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/BreachList/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class BreachListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public BreachListController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/BreachLists
        [HttpGet]
        public IEnumerable<BreachList> GetBreachLists()
        {
            return _context.BreachLists;
        }

        [HttpGet("GetArticles")]
        public IEnumerable<BreachArticle> GetArticles()
        {
            return _context.BreachArticles;
        }

        // GET: api/BreachLists/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBreachList([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breanchList = await _context.BreachLists.FindAsync(id);

            if (breanchList == null)
            {
                return NotFound();
            }

            return Ok(breanchList);
        }


        // POST: api/BreachList
        [HttpPost]
        public async Task<IActionResult> PostBreachList(int BreachListId, [FromBody] BreachList breachList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BreachList newBreachList = new BreachList();

            newBreachList.Fraction = breachList.Fraction;
            if(newBreachList.Fraction == null)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "falta agregar la fracción" });

            }
            newBreachList.Description = breachList.Description;
            if (newBreachList.Description ==  null)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar la descripcion" });

            }

            newBreachList.MinTimesFactor = breachList.MinTimesFactor;
            if(newBreachList.MinTimesFactor == 0)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "falta agregar factor minimo" });

            }

            newBreachList.MaxTimesFactor = breachList.MaxTimesFactor;
            if (newBreachList.MaxTimesFactor == 0)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar factor maximo" });

            }

            newBreachList.HaveBonification = breachList.HaveBonification;
            if (newBreachList.HaveBonification == false)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar la bonificacion" });

            }
            newBreachList.IsActive = breachList.IsActive;
            if(newBreachList.IsActive == false)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Activar el estado" });

            }
            newBreachList.BreachArticleId = breachList.BreachArticleId;
            if(newBreachList.BreachArticleId == 0)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar el id del ariculo" });

            }

            _context.BreachLists.Add(newBreachList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBreachLists", new { id = newBreachList.Id }, newBreachList);
        }

        // PUT: api/BreachList/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreachList([FromRoute] int id, [FromBody] BreachList breachList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != breachList.Id)
            {
                return BadRequest();
            }

            _context.Entry(breachList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreachListExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(breachList);
        }
        // DELETE: api/BreachList/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBreachList([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breachList = await _context.BreachLists.FindAsync(id);
            if (breachList == null)
            {
                return NotFound();
            }

            _context.BreachLists.Remove(breachList);
            await _context.SaveChangesAsync();

            return Ok(breachList);
        }



        private bool BreachListExist(int id)
        {
            return _context.BreachLists.Any(e => e.Id == id);
        }

    }
}
