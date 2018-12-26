using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class DiscountValidatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscountValidatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idAgreement}")]
        public IActionResult AssignDiscount([FromRoute] int idAgreement)
        {
            var sql = "exec billing_year @id_agreement, @error OUT";
            //var param1 = new SqlParameter("@id_agreement", System.Data.SqlDbType.Int);
            //var param2 = new SqlParameter("@error", System.Data.SqlDbType.VarChar,200);
            string error = string.Empty;
            //var sqlConnection = (SqlConnection)_context.Database.co;
            //BillingYear b = new BillingYear();
            //var b = _context.Database.ExecuteSqlCommand("billing_year @p0, @p1 OUT", param1, error);
            //var result = _context.Database.ExecuteSqlCommand(sql, param1, param2);
            //_context.Database.ExecuteSqlCommand(sql,);
            return Ok(_context.Database.ExecuteSqlCommand("billing_year @p0, @p1", idAgreement, error ));
            //return Ok(_context.Set().FromSql())
        }
    }
}