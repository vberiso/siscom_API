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
	[Route("api/BreachWarranty/")]
	[Produces("application/json")]
	[EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
	[ApiController]
	[Authorize]
	public class BreachWarrantyController : ControllerBase
	{

		private readonly ApplicationDbContext _context;


		public BreachWarrantyController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/Warranty
		[HttpGet]
		public IEnumerable<BreachWarranty> GetBreahWarranties()
		{
			return _context.BreachWarranties;
		}

		// GET: api/Warranty/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetBreachWarranty([FromRoute] int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var breachWarranty = await _context.BreachWarranties.FirstOrDefaultAsync(a=> a.BreachId == id);

			if (breachWarranty == null)
			{
				return NotFound();
			}

			return Ok(breachWarranty);
		}


		// POST: api/Warranty
		[HttpPost]
		public async Task<IActionResult> PostBreachWarranty(int BreachWarrantyId, [FromBody] BreachWarranty breachWarranty)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			_context.BreachWarranties.Add(breachWarranty);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetBreachWarranty", new { id = breachWarranty.BreachId }, breachWarranty);
		}

		// PUT: api/Warranty/1
		[HttpPut("{id}")]
		public async Task<IActionResult> PutBreachWarranty([FromRoute] int id, [FromBody] BreachWarranty Breachwarranty)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			

			_context.Entry(Breachwarranty).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BreachWarrantyExist(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return Ok(Breachwarranty);
		}

		// DELETE: api/Warranty/1
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBreachWarrranty([FromRoute] int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var breachWarranty = await _context.BreachWarranties.FindAsync(id);
			if (breachWarranty == null)
			{
				return NotFound();
			}

			_context.BreachWarranties.Remove(breachWarranty);
			await _context.SaveChangesAsync();

			return Ok(breachWarranty);
		}


		private bool BreachWarrantyExist(int id)
		{
			return _context.BreachWarranties.Any(e => e.BreachId == id);
		}
	}
}
