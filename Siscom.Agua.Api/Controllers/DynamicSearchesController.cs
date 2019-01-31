using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;


namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicSearchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DynamicSearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DynamicSearches
        [HttpGet]
        public IActionResult Get()
        {
            
            
            return Ok();
        }

        // GET: api/DynamicSearches/5
        [HttpGet("Padron")]
        public IEnumerable<Agreement> Gett(PadronFilter filter)
        {
            DateTime star;
            DateTime end;
            DateTime.TryParse(filter.StratDate, out star);
            DateTime.TryParse(filter.EndDate, out end);

            if(filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume && 
                                            (from d in x.Debts
                                            where d.Status == "ED001" || d.Status == "ED004"
                                            select d).Sum(z => z.Amount) > filter.Amount    &&
                                            x.TypeIntakeId == filter.TypeIntake     &&
                                            x.TypeServiceId == filter.TypeService   &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }

            return null;
        }

        // POST: api/DynamicSearches
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/DynamicSearches/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
    }
}
