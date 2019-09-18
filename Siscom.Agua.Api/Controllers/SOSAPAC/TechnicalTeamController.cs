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
    public class TechnicalTeamController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public TechnicalTeamController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("Teams/{id?}")]
        public async Task<IActionResult> GetTeams([FromRoute] string id = null)
        {
            if (id == null)
            {
                var Teams = _context.TechnicalTeams.ToList();
                return Ok(Teams);
            }
            var Team = _context.TechnicalTeams.Where(x => x.Id.ToString() == id).First();
            return Ok(Team);

        }

        [HttpPost("Teams")]
        public async Task<IActionResult> SetTeams([FromBody] TechnicalTeam data)
        {
            try
            {

                _context.TechnicalTeams.Add(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new {msg = "Los datos se guardaron correctamente" , ID = data.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { msg = ex.Message});
            }
            
        }

        [HttpPost("UpdateTeams/{id}")]
        public async Task<IActionResult> UpdateTeams([FromRoute] int id, [FromBody] TechnicalTeam data)
        {
            try
            {
                var team = _context.TechnicalTeams.Where(x => x.Id == id).First();
                team.Description = data.Description;
                team.Name = data.Name;
                team.IsActive = data.IsActive;
                _context.TechnicalTeams.Update(team);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("DeleteTeams/{id}")]
        public async Task<IActionResult> DeleteTeams([FromRoute] int id)
        {
            try
            {
                var team = _context.TechnicalTeams.Where(x => x.Id == id).First();
                
                _context.TechnicalTeams.Remove(team);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "El elemento se elimino correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { msg = ex.Message });
            }

        }


    }
}