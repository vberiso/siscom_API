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
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class CancelRechargesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CancelRechargesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{AgreementId}")]
        public async Task<IActionResult> CancelRecharges([FromRoute]int AgreementId)
        {
            string error = string.Empty;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "cancel_recharges";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@id_agreement", AgreementId));
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@error",
                    DbType = DbType.String,
                    Size = 200,
                    Direction = ParameterDirection.Output
                });
                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    if (!result.HasRows)
                    {
                        error = command.Parameters["@error"].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Message = "Se han Cancelado los recargos satisfactoriamente" });
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Problemas para cancelar recargos [ "+ error + "]"});
                    }
                }
            }
        }
    }
}