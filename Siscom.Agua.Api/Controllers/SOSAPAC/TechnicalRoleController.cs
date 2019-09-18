using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicalRoleController : ControllerBase
    {



        private readonly ApplicationDbContext _context;

        public TechnicalRoleController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("Roles/{id?}")]
        public async Task<IActionResult> GetRoles([FromRoute] string id = null)
        {
            if (id == null)
            {
                var Teams = _context.TechnicalRoles.ToList();
                return Ok(Teams);
            }
            var Team = _context.TechnicalRoles.Where(x => x.Id.ToString() == id).First();
            return Ok(Team);
        }

        [HttpPost("Roles")]
        public async Task<IActionResult> SetRoles([FromBody] TechnicalRole data)
        {
            try
            {
                _context.TechnicalRoles.Add(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se guardaron correctamente" , ID= data.Id});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("UpdateRoles/{id}")]
        public async Task<IActionResult> UpdateRoles([FromRoute] int id, [FromBody] TechnicalRole data)
        {
            try
            {
                var team = _context.TechnicalRoles.Where(x => x.Id == id).First();  
                team.Name = data.Name;
                team.IsActive = data.IsActive;
                _context.TechnicalRoles.Update(team);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("DeleteRoles/{id}")]
        public async Task<IActionResult> DeleteRoles([FromRoute] int id)
        {
            try
            {
                var team = _context.TechnicalRoles.Where(x => x.Id == id).First();

                _context.TechnicalRoles.Remove(team);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "El elemento se elimino correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }
    }
}