using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            //var expectedName = ExpressionHelper.WrappedConstant(id);
            //var param = Expression.Parameter(typeof(Agreement));
            //var nameProperty = Expression.Property(param, typeof(Agreement).GetProperty("Id"));
            //var equal = Expression.Equal(nameProperty, expectedName);
            //var filter = Expression.Lambda(equal, param);

            //var agreement = _context.Agreements.Where(filter);

            var query = _context.Agreements.Where("Id == @0", id);

            return Ok(query);
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
    }
}


public static class ExpressionHelper
{
    public static MemberExpression WrappedConstant<TValue>(TValue value)
    {
        var wrapper = new WrappedObj<TValue>(value);
        return Expression.Property(
            Expression.Constant(wrapper),
            typeof(WrappedObj<TValue>).GetProperty("Value"));
    }

    private class WrappedObj<TValue>
    {
        public TValue Value { get; set; }
        public WrappedObj(TValue value)
        {
            this.Value = value;
        }
    }
}