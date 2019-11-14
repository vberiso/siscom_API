using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CondonationCampaingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CondonationCampaingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCondonation()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.CondonationCampaings.Where(c => c.IsActive == true && c.StartDate < DateTime.Now && c.EndDate > DateTime.Now).ToListAsync();

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }


        [HttpPost("{idAgreement}/{idCondonation}")]
        public async Task<IActionResult> PostCondonation([FromRoute] int idAgreement, [FromRoute] int idCondonation)
        {
            string error = string.Empty;            
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[Campaing_condonation]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", idAgreement));
                    command.Parameters.Add(new SqlParameter("@id_con_camp", idCondonation));                    
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
                        if (result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                        error = command.Parameters["@error"].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo realizar la condonación de recargos por las siguientes razones: [{error}]") });
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = idAgreement.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para cancelar recargos" });
            }

        }

    }
}