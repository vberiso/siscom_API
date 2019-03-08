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
using Siscom.Agua.Api.Services.Extension;
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

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    newBreachList.Fraction = breachList.Fraction;
                    if (newBreachList.Fraction == null)
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Error = "falta agregar la fracción" });

                    }
                    newBreachList.Description = breachList.Description;
                    if (newBreachList.Description == null)
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar la descripcion" });

                    }

                    newBreachList.MinTimesFactor = breachList.MinTimesFactor;
                    if (newBreachList.MinTimesFactor == 0)
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
                    if (newBreachList.IsActive == false)
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Error = "Activar el estado" });

                    }
                    newBreachList.BreachArticleId = breachList.BreachArticleId;
                    if (newBreachList.BreachArticleId == 0)
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Error = "Falta agregar el id del ariculo" });

                    }

                    _context.BreachLists.Add(newBreachList);
                    await _context.SaveChangesAsync();

                    scope.Complete();
                }


            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breachList);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para crear lista de infracción" });

            }

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


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

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

                }


            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breachList);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar lista de infracción" });
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
