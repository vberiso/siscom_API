using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonCatalogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReasonCatalogController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("index/{id?}")]
        public async Task<IActionResult> GetReasonCatalog([FromRoute] string id = null, [FromQuery] string type= null)
        {
            try
            {
                if (id == null)
                {
                    System.Linq.IQueryable<ReasonCatalog> query = _context.ReasonCatalog;
                    if (type != null)
                    {
                        query = query.Where(x => x.Type == type && x.IsActive == true);
                    }
                    var ReasonCatalogs = query.ToList();
                    return Ok(ReasonCatalogs);
                }
                var ReasonCatalog = _context.ReasonCatalog.Where(x => x.Id.ToString() == id).First();
                return Ok(ReasonCatalog);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message});
            }
        }

        [HttpPost("store")]
        public async Task<IActionResult> StoreReasonCatalog([FromBody] ReasonCatalog data)
        {
            try
            {
                _context.ReasonCatalog.Add(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se guardaron correctamente", ID = data.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateReasonCatlog([FromRoute] int id, [FromBody] ReasonCatalog data)
        {
            try
            {
                data.Id = id;
                _context.ReasonCatalog.Update(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> delete([FromRoute] int id)
        {
            try
            {

                ReasonCatalog ReasonCatalog = _context.ReasonCatalog.Find(id);
                _context.ReasonCatalog.Remove(ReasonCatalog);
                _context.SaveChanges();
                return Ok(new {msg = "Registro eliminado correctamente"});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message});
            }
        }

    }
}