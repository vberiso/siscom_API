using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ValvulasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ValvulasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{indice}/{regs?}")]
        public async Task<IActionResult> GetValves([FromRoute] int indice, [FromRoute] int regs = 10)
        {
            try
            {
                var valves = await _context.ValvulaControls
                                    .Include(x => x.ValveIncidents)
                                    .Include(x => x.ValveOperations)
                                    .Skip((indice - 1) * regs).Take(regs).ToListAsync();

                var total = _context.ValvulaControls.Count();

                return Ok(new List<object>() { valves, total });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetValve([FromRoute] int id)
        {
            try
            {
                var valve = await _context.ValvulaControls
                                    .Include(x => x.ValveIncidents)
                                    .Include(x => x.ValveOperations)
                                    .FirstOrDefaultAsync(v => v.Id == id);

                if (valve == null)
                    return NotFound();
                else
                    return Ok(valve);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutValve([FromRoute] int id, [FromBody] ValvulaControl valvulaControl)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != valvulaControl.Id)
                    return BadRequest("Inconsistencia en la información");


                var tmpValvula = await _context.ValvulaControls.FindAsync(id);
                if (tmpValvula == null)
                    return NotFound();

                tmpValvula.Description = valvulaControl.Description;
                tmpValvula.Reference = valvulaControl.Reference;
                tmpValvula.Latitude = valvulaControl.Latitude;
                tmpValvula.Longitude = valvulaControl.Longitude;
                tmpValvula.Type = valvulaControl.Type;
                tmpValvula.IsActive = valvulaControl.IsActive;
                tmpValvula.Diameter = valvulaControl.Diameter;
                tmpValvula.HydraulicCircuit = valvulaControl.HydraulicCircuit;
                tmpValvula.PhysicalState = valvulaControl.PhysicalState;
                tmpValvula.ActualState = valvulaControl.ActualState;
                tmpValvula.LastServiceDate = valvulaControl.LastServiceDate;

                _context.Entry(tmpValvula).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
                //return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Problemas al consultar los registros {0}", ex.Message) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostValves([FromBody] ValvulaControl valvulasControl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.ValvulaControls.Add(valvulasControl);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteValves([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest(ModelState);

                var tmpValvula = await _context.ValvulaControls.FindAsync(id);
                if (tmpValvula == null)
                    return NotFound();

                _context.ValvulaControls.Remove(tmpValvula);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Catalogos")]
        public async Task<IActionResult> GetCatalogos()
        {
            try
            {
                var estadosFisico = await _context.Lists.Where(f => f.IsActive == true && f.GroupListsId == 10).ToListAsync();
                var estadosActual = await _context.Lists.Where(f => f.IsActive == true && f.GroupListsId == 11).ToListAsync();
                List<object> catalogos = new List<object>() { estadosFisico, estadosActual };
                return Ok(catalogos);
            }
            catch (Exception ex)
            {
                return NotFound(new { error = "No se encontraron los catalogos" });
            }
        }
    }
}