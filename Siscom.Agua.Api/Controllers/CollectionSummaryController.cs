using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class CollectionSummaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CollectionSummaryController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("IncomeReport/{dateInicio}/{dateFin}")]
        public async Task<IActionResult> Get([FromRoute] string dateInicio, string dateFin)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "collectionSummary";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaIni", dateInicio));
                command.Parameters.Add(new SqlParameter("@fechaFin", dateFin));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);
        }

        [HttpGet("TotalCollection/{dateInicio}/{dateFin}")]
        public async Task<IActionResult> GetTotal([FromRoute] string dateInicio, string dateFin)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "totalCollection";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaIni", dateInicio));
                command.Parameters.Add(new SqlParameter("@fechaFin", dateFin));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(result);
                }
            }
            return Ok(dataTable);
        }
    }
}