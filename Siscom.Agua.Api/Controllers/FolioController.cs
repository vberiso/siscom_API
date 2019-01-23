using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    // <summary>
    /// End Points Folio
    /// </summary>
    [Route("api/Folio")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class FolioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FolioController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Folio
        /// </summary>
        /// <returns></returns>
        // GET: api/Folio
        [HttpGet]
        public IEnumerable<Folio> GetFolio()
        {
            return _context.Folios;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Folio which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>Folio Model</returns>
        // GET: api/Folio/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Folio = await _context.Folios.FindAsync(id);

            if (Folio == null)
            {
                return NotFound();
            }

            return Ok(Folio);
        }

        /// <summary>
        /// This will provide update for the specific ID,
        /// </summary>
        /// <param name="id">id from route (URL)</param>
        /// <param name="pfolio">Model Folio</param>
        /// <returns></returns>
        // PUT: api/Folio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolio([FromRoute] int id, [FromBody] Folio pfolio)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != pfolio.Id)
                return BadRequest();

            if (!Validate(pfolio))
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });

            var branchOffice = await _context.BranchOffices.FindAsync(pfolio.BranchOfficeId);
            if (branchOffice == null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("La sucursal no existe") });
            if (!branchOffice.IsActive)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("La sucursal no está activa") });

            if (pfolio.IsActive == 1)
            {
                if (await _context.Folios
                                 .Where(x => x.BranchOfficeId == pfolio.BranchOfficeId &&
                                             x.IsActive == 1)
                                 .FirstOrDefaultAsync() != null)
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("Ya existe una serie activa para esta sucursal") });
            }

            if (pfolio.IsActive == 0)
            {
                if(branchOffice.DontClose)
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("No se puede deshabilitar folios mientras la sucursal esté operando") });

                if (DateTime.UtcNow.ToLocalTime().Hour > branchOffice.Opening.Hour && DateTime.UtcNow.ToLocalTime().Hour < branchOffice.Closing.Hour)
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("No se puede deshabilitar folios mientras la sucursal esté operando") });
            }

            Folio folio = new Folio();
            folio = await _context.Folios
                                 .Where(x => x.Id == pfolio.Id)
                                 .FirstOrDefaultAsync();
            if (folio == null)
                return NotFound();

            folio.IsActive = pfolio.IsActive;
            _context.Entry(folio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)TypeError.Code.Ok, new { Message = string.Format("Modificación realizada con éxito") });
        }

        /// <summary>
        /// This will provide capability add new Folio 
        /// </summary>
        /// <param name="pfolio">Model Folio</param>
        /// <returns>New Folio added</returns>
        // POST: api/Folio
        [HttpPost]
        public async Task<IActionResult> PostFolio([FromBody] Folio pfolio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pfolio))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

           

            if(await _context.Folios
                             .Where(x => x.BranchOfficeId == pfolio.BranchOfficeId &&
                                         x.IsActive == 1)
                            .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("Ya existe una serie activa para esta sucursal") });


           
            var folios = await _context.Folios
                                       .Where(x => x.BranchOfficeId == pfolio.BranchOfficeId &&
                                                   x.Range == pfolio.Range)
                                       .ToListAsync();

            bool _conflic = false;

            folios.ForEach(x => {               
                    if (pfolio.Secuential< x.Secuential)
                        _conflic = true;                    
            });

            if(_conflic)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Folio fuera de rango" });             

            _context.Folios.Add(pfolio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolio", new { id = pfolio.Id },pfolio);
        }


        private bool FolioExists(int id)
        {
            return _context.Folios.Any(e => e.Id == id);
        }

        private bool Validate(Folio folio)
        {
            if (folio.Initial == 0)
                return false;
            if (folio.Secuential == 0)
                return false;
            if (folio.BranchOfficeId == 0)
                return false;
            return true;
        }
    }
}
