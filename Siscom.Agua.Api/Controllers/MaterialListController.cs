using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    public class MaterialListController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public MaterialListController(ApplicationDbContext context)
        {
            this._context = context;
        }
        // GET: api/MaterialList
        [HttpGet]
        public async Task<IEnumerable<MaterialList>> Get()
        {
            return await _context.MaterialLists
                .Include(x => x.UnitMeasurements)
                .Where(x => x.IsActive).ToListAsync();
        }

        // GET: api/MaterialList/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<MaterialList> Get(int id)
        {
            return await _context.MaterialLists
                .Include(x => x.UnitMeasurements)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        //-------------------Materiales
        [HttpGet("listado/{id?}")]
        public async Task<IActionResult> GetMateriales([FromRoute] int id = 0)
        {
            try
            {
                System.Linq.IQueryable<MaterialList> query = _context.MaterialLists.AsQueryable();

                if (id != 0)
                {
                    query = query.Where(x => x.Id == id);
                }

                var materialLists = query.ToList();
                return Ok(materialLists);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> PostMateriales([FromBody] MaterialList materialList)
        {
            try
            {
                var tmpMaterialList = await _context.MaterialLists.FirstOrDefaultAsync(m => m.Name.Equals(materialList.Name) && m.Code.Equals(materialList.Code));
                if (tmpMaterialList != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { error = "El material ya existe" });
                }

                await _context.AddAsync(materialList);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message });
            }
        }

        [HttpPut()]
        public async Task<IActionResult> PutMateriales([FromBody] MaterialList materialList)
        {
            try
            {
                var tmpMaterialList = await _context.MaterialLists.FirstOrDefaultAsync(m => m.Id == materialList.Id);
                if (tmpMaterialList == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { error = "El material no existe" });
                }

                tmpMaterialList.Name = materialList.Name;
                tmpMaterialList.Code = materialList.Code;
                tmpMaterialList.IsActive = materialList.IsActive;

                _context.MaterialLists.Update(tmpMaterialList);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message });
            }
        }

        [HttpDelete()]
        public async Task<ActionResult> deleteMateriales([FromBody] int id)
        {
            try
            {
                var tmpMaterialList = await _context.MaterialLists.FirstOrDefaultAsync(m => m.Id == id);
                if (tmpMaterialList == null)
                {
                    return StatusCode(StatusCodes.Status204NoContent, new { error = "el material no existe." });
                }

                _context.MaterialLists.Remove(tmpMaterialList);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

    }
}
