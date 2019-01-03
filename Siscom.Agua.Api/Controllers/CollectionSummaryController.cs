using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
    [ApiController]
    [Authorize]
    public class CollectionSummaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CollectionSummaryController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("{dateInicio}/{dateFin}")]
        public async Task<IActionResult> get([FromRoute] string dateInicio, string dateFin)
        {
            string error = string.Empty;
            List<CollectionSummaryVM> entities = null;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "collectionSummary";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fechaIni", dateInicio));
                command.Parameters.Add(new SqlParameter("@fechaFin", dateFin));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    entities = new List<CollectionSummaryVM>();
                    var x = result.Read();
                    while (result.Read())
                    {
                        var T = new CollectionSummaryVM();
                        T.PaymentDate = result[0].ToString();
                        T.Account = result[1].ToString();
                        T.Total = Convert.ToDecimal(result[2].ToString());
                        T.BrancOffice = result[3].ToString();
                        T.PayMethod = result[4].ToString();
                        T.OriginPayment = result[5].ToString();
                        T.External_Origin_Payment = result[6].ToString();
                        entities.Add(T);
                    }
                }
            }
            return Ok(entities);
        }
    }
}