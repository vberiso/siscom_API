using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivosInspaccionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;
        public MotivosInspaccionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpGet("index/{id?}")]
        public async Task<IActionResult> GetMotivosInsp([FromRoute] string id = null, [FromQuery] bool isEjecute = false)
        {
            try
            {
                if (id == null)
                {
                    System.Linq.IQueryable<InspectionFine> query = _context.InspectionFines;
                    if (isEjecute)
                    {
                        query = query.Where(x => x.IsActive == true);
                    }
                   
                    var ReasonCatalogs = query.ToList();
                    return Ok(ReasonCatalogs);
                }
                var ReasonCatalog = _context.InspectionFines.Where(x => x.Id.ToString() == id).First();
                return Ok(ReasonCatalog);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message });
            }
        }

        [HttpPost("store")]
        public async Task<IActionResult> StoreReasonCatalog([FromBody] InspectionFine data)
        {
            try
            {
                _context.InspectionFines.Add(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se guardaron correctamente", ID = data.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateReasonCatlog([FromRoute] int id, [FromBody] InspectionFine data)
        {
            try
            {
                data.Id = id;
                _context.InspectionFines.Update(data);
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

                InspectionFine InspectionFine = _context.InspectionFines.Find(id);
                _context.InspectionFines.Remove(InspectionFine);
                _context.SaveChanges();
                return Ok(new { msg = "Registro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

    }
}