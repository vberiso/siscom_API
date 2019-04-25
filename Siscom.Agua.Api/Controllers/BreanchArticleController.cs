using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
namespace Siscom.Agua.Api.Controllers
{
    [Route("api/BreachArticle/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    _context.BreachArticles.Add(breachArticle);
                    await _context.SaveChangesAsync();


                    scope.Complete();

                    return CreatedAtAction("GetBreachArticle", new { id = breachArticle.Id }, breachArticle);


                }


            }
            catch(Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breachArticle);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para crear articulo de infracción" });


            }
        }


        // PUT: api/BreachArticle/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreachArticle([FromRoute] int id, [FromBody] BreachArticle breachArticle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
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
                    scope.Complete();
                    return Ok(breachArticle);
                }

            }
            catch(Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breachArticle);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para generar articulo de infraccion" });

            }


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
