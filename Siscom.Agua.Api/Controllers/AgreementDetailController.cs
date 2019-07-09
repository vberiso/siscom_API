//AgreementDetailController

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/AgreementDetails/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class AgreementDetailController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public AgreementDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Warranty
        [HttpGet]
        public IEnumerable<AgreementDetail> GetAgreementDetails()
        {
            return _context.AgreementDetails;
        }

        // GET: api/Warranty/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgreementDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var det = await _context.AgreementDetails.FindAsync(id);

            if (det == null)
            {
                return NotFound();
            }

            return Ok(det);
        }


        // POST: api/Warranty
        [HttpPost]
        public async Task<IActionResult> PostAgreementDetail(int AgreementId, [FromBody] AgreementDetail detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.AgreementDetails.Add(detail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAgreementDetail", new { id = detail.Id }, detail);
        }

        // PUT: api/Warranty/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgreementDetail([FromRoute] int id, [FromBody] AgreementDetail detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != detail.Id)
            {
                return BadRequest();
            }

            _context.Entry(detail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetailExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(detail);
        }

        // DELETE: api/Warranty/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgreementDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var det = await _context.AgreementDetails.FindAsync(id);
            if (det == null)
            {
                return NotFound();
            }

            _context.AgreementDetails.Remove(det);
            await _context.SaveChangesAsync();

            return Ok(det);
        }


        private bool DetailExist(int id)
        {
            return _context.AgreementDetails.Any(e => e.Id == id);
        }
    }
}

