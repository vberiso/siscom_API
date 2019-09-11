using System;
using System.Collections.Generic;
using System.Data;

using System.IO;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Exceptions;
using Siscom.Agua.Api.runSp;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;

using Siscom.Agua.DAL.Models;


using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Siscom.Agua.Api.Controllers
{

    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class StoreProcedureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        string error = string.Empty;

        public StoreProcedureController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("runAccrualPeriod/{id_agreement}/{start_month}/{end_month}/{from_year}/{simulate}")]
        public async Task<IActionResult> ExectSpBillingPeriod(/*[FromBody] int user_id, [FromBody] string observaciones,*/ [FromRoute] int id_agreement, [FromRoute] int start_month, [FromRoute] int end_month, [FromRoute] int from_year, [FromRoute] int simulate)
        {

            int mesesEfectuados = 0;
            object result = null;
            for (int i = start_month; i <= end_month; i++)
            {
                List<SPParameters> parameters = new List<SPParameters> {
                new SPParameters{Key ="id_agreement", Value = id_agreement.ToString() },
                new SPParameters{Key ="from_month", Value = i.ToString() },
                new SPParameters{Key ="from_year", Value = from_year.ToString() },
                new SPParameters{Key ="simulate", Value = simulate.ToString() },
                new SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}


                };


                try
                {
                    result = await new RunSP(this, _context).runProcedureNT("accrual_period", parameters);
                    parameters = null;
                    mesesEfectuados++;

                }
                catch (RunSpException e)
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}"), Message = $"Se regiostrarón los {mesesEfectuados} primeros meses" });
                }
                catch (Exception e)
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}", Message = $"Se regiostrarón los {mesesEfectuados} primeros meses" });
                }
            }

            if (simulate != 1)
            {
                var body = "";

                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEnd();

                }
                if (body != "")
                {
                    var jObject = JObject.Parse(body);

                    string user_id = jObject["user_id"].ToString();
                    ApplicationUser user = await _context.Users.FindAsync(user_id);
                    var error = JObject.Parse(JsonConvert.SerializeObject(result));

                    bool is_error = !string.IsNullOrEmpty(error["paramsOut"][0]["Value"].ToString().Trim());


                    if (!is_error && !createCommentForAgremment(id_agreement, jObject["descripcion"].ToString(), user))
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Message = $"Se registrarón todos, pero no se pudo registrar el comentario", data = result });
                    }
                }
            }

            return StatusCode((int)TypeError.Code.Ok, new { Message = $"Se registrarón todos los meses correctamente", data = result });
        }


        [HttpPost("runAccrualPeriodNow/{id_agreement}/{from_month}/{from_year}/{status}")]
        public async Task<IActionResult> ExectSpAccrualPeriodNow([FromRoute] int id_agreement, [FromRoute] int from_year, [FromRoute] int from_month, [FromRoute] string status)
        {
            var body = "";

            using (var reader = new StreamReader(Request.Body))
            {
                body = reader.ReadToEnd();

            }
            var jObject = JObject.Parse(body);


            object result = null;

            List<SPParameters> parameters = new List<SPParameters> {
                new SPParameters{Key ="id_agreement", Value = id_agreement.ToString() },
                new SPParameters{Key ="from_month", Value = from_month.ToString() },
                new SPParameters{Key ="from_year", Value = from_year.ToString() },
                new SPParameters{Key ="status", Size=5, Value = status.ToString() },
                new SPParameters { Key = "error", Size=200, Direccion= ParameterDirection.InputOutput, DbType= DbType.String, Value =""}


                };
            try
            {
                result = await new RunSP(this, _context).runProcedureNT("accrual_period_now", parameters);
                parameters = null;

            }
            catch (RunSpException e)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}"), Message = $"Error interno del servidorNo se pudo realizar la transacción" });
            }
            catch (Exception e)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}", Message = $"Error interno del servidorNo se pudo realizar la transacción" });
            }
            string user_id = jObject["user_id"].ToString();
            ApplicationUser user = await _context.Users.FindAsync(user_id);
            var error = JObject.Parse(JsonConvert.SerializeObject(result));

            bool is_error = !string.IsNullOrEmpty(error["paramsOut"][0]["Value"].ToString().Trim());


            if (!is_error && !createCommentForAgremment(id_agreement, jObject["descripcion"].ToString(), user))
            {
                return StatusCode((int)TypeError.Code.Ok, new { Message = $"Se registro el periodo, pero no se pudo registrar el comentario", data = result });
            }


            return StatusCode((int)TypeError.Code.Ok, new { Message = $"Transacción exitosa", data = result });

        }

        private bool createCommentForAgremment(int id_agreement, string observacion, ApplicationUser user)
        {
            try
            {



                AgreementComment AgreementComment = new AgreementComment()
                {
                    AgreementId = id_agreement,
                    DateIn = DateTime.Now,
                    DateOut = DateTime.Now.AddDays(5),
                    Observation = observacion,
                    IsVisible = true,
                    UserId = user.Id.ToString(),
                    UserName = user.Name
                };
                _context.AgreementComments.Add(AgreementComment);
                _context.SaveChanges();



                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// End Points Para formatos de agua
        /// </summary>

        [HttpPost("runFormatos/{formato}")]
        public async Task<IActionResult> ExectSpFormatos([FromRoute] string formato)
        {

            var body = "";
            List<object> Lresult = null;
            object result = null;


            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEnd();

                }
                List<SPParameters> parameters;
               // var saa = JObject.Parse(body);
                if (string.IsNullOrEmpty(body))
                {
                   parameters = new List<SPParameters>();
                }
                else
                {
                    parameters = JsonConvert.DeserializeObject<List<SPParameters>>(body);
                }
                parameters.Add(new SPParameters { Key = "error", Size = 200, Direccion = ParameterDirection.InputOutput, DbType = DbType.String, Value = "" });


                if (formato == "fza_formato3" || formato == "fza_formato2")
                {


                    int index = parameters.FindIndex(x => x.Key.Equals("mesPresentar"));
                    if (index != -1)
                    {
                        parameters[index].Key = "mesPresentarInicio";
                        parameters.Add(new SPParameters { Key = "mesPresentarFin", Value = parameters[index].Value, DbType = DbType.Int32 });
                        //parameters.Add(new SPParameters{ Key = "formato", Value = formato, DbType = DbType.String,Size = 15 });

                    }
                    if (formato == "fza_formato3")
                    {
                        parameters.Add(new SPParameters { Key = "onlyPaid", Value = "2", DbType = DbType.Int32 });

                    }
                    result = await new RunSP(this, _context).runProcedureNT(formato, parameters, 1);
                    if (formato == "fza_formato3")
                    {
                        Lresult = new List<object>();
                        Lresult.Add(result);
                        if (index != -1)
                        {
                            parameters[index].Value = "01";

                            parameters.Last().Value = "1";
                            //parameters.Add(new SPParameters { Key = "mesPresentarFin", Value = parameters[index].Value, DbType = DbType.Int32 });

                        }
                        result = await new RunSP(this, _context).runProcedureNT(formato, parameters, 1);
                        Lresult.Add(result);
                        return StatusCode((int)TypeError.Code.Ok, new { data = Lresult });
                    }
                }
                else
                {
                    result = await new RunSP(this, _context).runProcedureNT(formato, parameters, 1);
                }

                parameters = null;
            }
            catch (RunSpException e)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}") });
            }
            catch (Exception e)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}" });
            }




            return StatusCode((int)TypeError.Code.Ok, new { data = result });
        }

        [HttpPost("RunFormato23Desgloce")]
        public async Task<IActionResult> ExectSpFzaFormato23Desgloce()
        {
            var body = "";
        
            object result = null;

            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEnd();

                }

                List<SPParameters> parameters = JsonConvert.DeserializeObject<List<SPParameters>>(body);
                parameters.Add(new SPParameters { Key = "error", Size = 200, Direccion = ParameterDirection.InputOutput, DbType = DbType.String, Value = "" });
                result = await new RunSP(this, _context).runProcedureNT("fza_formato2_3_desgloce", parameters, 1);

            }
            catch (RunSpException e)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}") });
            }
            catch (Exception e)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}" });
            }

            return StatusCode((int)TypeError.Code.Ok, new { data = result });
        }


    }





}