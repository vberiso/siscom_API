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
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class GenerateDiscountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenerateDiscountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscount([FromBody] PushNotifications discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _context.PushNotifications.AddAsync(discount);
                await _context.SaveChangesAsync();
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message= ExectSP(discount) });
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(discount);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar el descuento, favor de verificar " });
            }
           


            //if (discount.Type == "NOT01")
            //{

            //}
            return Ok();
        }

        private async Task<string> ExectSP(PushNotifications discount)
        {
            string error = string.Empty;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "billing_Adjusment";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@id_agreement", discount.AgreementId));
                command.Parameters.Add(new SqlParameter("@id_debt", discount.DebtId));
                command.Parameters.Add(new SqlParameter("@porcent", discount.Porcentage));
                command.Parameters.Add(new SqlParameter("@amount", discount.Amount));
                command.Parameters.Add(new SqlParameter("@text_discount", discount.Reason));
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
                }
                if (!string.IsNullOrEmpty(error))
                {
                    return "Se Genero el descuento" ;
                }
                else
                {
                    return error ;
                }
            }
        }
    }
}