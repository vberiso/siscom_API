using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Group/{id}")]
        public async Task<IActionResult> GetTypeGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = await _context.Types
                                     .Where(x => x.GroupTypeId == id).ToListAsync();

            if (type == null)
            {
                return NotFound();
            }

            return Ok(type);
        }

    }
}