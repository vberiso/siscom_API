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

        //// POST: api/MaterialList
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/MaterialList/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
