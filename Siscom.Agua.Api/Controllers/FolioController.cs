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
        /// <param name="pfolio">Model FolioVM</param>
        /// <returns></returns>
        // PUT: api/Folio/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolio([FromRoute] int id, [FromBody] FolioVM pfolio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pfolio.Id)
            {
                return BadRequest();
            }

            if (!Validate(pfolio))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            Folio folio = new Folio();
            folio.Initial = pfolio.Folio;
            folio.IsActive = pfolio.IsActive;
            folio.Range = pfolio.Range;
            folio.Secuential = pfolio.Folio;
            folio.BranchOffice = await _context.BranchOffices.FindAsync(pfolio.BranchOffice);
           
            _context.Entry(folio).State = EntityState.Modified;

            try
            {
                if (await _context.Folios.Where(x => x.Id == pfolio.Id &&
                                                     x.Secuential == pfolio.Folio &&
                                                     x.Range == pfolio.Range &&
                                                     x.BranchOffice.Id == pfolio.BranchOffice)
                                           .FirstOrDefaultAsync() != null)
                     await _context.SaveChangesAsync();
                else
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = string.Format("Sólo se puede modificar el estado") });
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
        /// <param name="pfolio">Model FolioVM</param>
        /// <returns>New Folio added</returns>
        // POST: api/Folio
        [HttpPost]
        public async Task<IActionResult> PostFolio([FromBody] FolioVM pfolio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pfolio))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            if (await _context.Folios.Where(x => x.Range == pfolio.Range &&
                                                 x.BranchOffice.Id != pfolio.BranchOffice)
                                           .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "La serie de folios ya ha sido dada de alta previamente" });
            }

            if (await _context.Folios.Where(x => x.BranchOffice.Id == pfolio.BranchOffice &&
                                                 x.IsActive==1)
                                           .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "La sucursal ya cuenta con una serie de folios" });
            }


            if (await _context.Folios.Where(x => x.BranchOffice.Id== pfolio.BranchOffice &&
                                                 x.Range == pfolio.Range &&
                                                 x.Secuential <= pfolio.Folio  )
                                           .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Folio fuera de rango" });
            }

            Folio folio = new Folio();
            folio.Initial = pfolio.Folio;
            folio.IsActive = pfolio.IsActive;
            folio.Range = pfolio.Range;
            folio.Secuential = pfolio.Folio;
            folio.BranchOffice = await _context.BranchOffices.FindAsync(pfolio.BranchOffice);            

            _context.Folios.Add(folio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolio", new { id = folio.Id }, folio);
        }


        private bool FolioExists(int id)
        {
            return _context.Folios.Any(e => e.Id == id);
        }

        private bool Validate(FolioVM folio)
        {
            if (folio.Folio == 0)
                return false;
            if (folio.BranchOffice == 0)
                return false;
            return true;
        }
    }
}
