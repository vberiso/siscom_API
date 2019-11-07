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

                        if (index != -1)
                        {
                            // parameters[index].Value = "01";

                            parameters.Last().Value = "1";
                            //parameters.Add(new SPParameters { Key = "mesPresentarFin", Value = parameters[index].Value, DbType = DbType.Int32 });

                        }
                        Lresult.Add(result);
                        result = await new RunSP(this, _context).runProcedureNT(formato, parameters, 1);
                        Lresult.Add(result);

                        string mes = parameters.Where(x => x.Key == "mesPresentarFin").FirstOrDefault()?.Value;
                        string year = parameters.Where(x => x.Key == "yearPeriodo").FirstOrDefault()?.Value;
                        string newMes = mes;
                        string newYear = year;
                        if (int.Parse(mes) == 1)
                        {
                            newMes = "12";
                            newYear = (int.Parse(year) - 1).ToString();
                        }
                        else
                        {
                            newMes = (int.Parse(mes) - 1).ToString();
                        }
                        bool isMedido = parameters.Where(x => x.Key == "TypeServiceId" && x.Value == "1").FirstOrDefault() != null ? true : false;




                        var nextAccumulated = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "domestico" && x.Mes.ToString() == mes && x.Year.ToString() == year).FirstOrDefault()?.Accumulated;
                        if (nextAccumulated != null)
                        {
                            newMes = mes;
                            newYear = year;
                        }

                        var AccumulatedD = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                        var AccumulatedND = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "no_domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                        var AccumulatedO = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "otros" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;


                        var AccumulatedeD = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "edomestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                        var AccumulatedeND = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "eno_domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                        var AccumulatedeO = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "eotros" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;


                        newMes = (int.Parse(mes) + 1).ToString();


                        if (nextAccumulated == null)
                        {
                            bool create = false;
                            if (int.Parse(year) == DateTime.Now.Year && int.Parse(mes) < DateTime.Now.Month)
                            {
                                create = true;
                            }
                            else if (int.Parse(year) < DateTime.Now.Year && int.Parse(mes) == 12 && DateTime.Now.Month == 1)
                            {
                                create = true;
                            }
                            if (create)
                            {
                                List<object> agreements = JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(Lresult.First()));
                                List<object> agreementse = JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(Lresult.Last()));
                                SetAccumulated(year, mes, isMedido, new List<List<object>>() { agreements, agreementse });
                            }

                        }
                        else
                        {
                            Lresult = new List<object>() { new List<object>(), new List<object>() };
                            //return StatusCode((int)TypeError.Code.Ok, new { data = Lresult });
                        }

                        Lresult.Add(new List<object>() { AccumulatedD == null ? 0 : AccumulatedD, AccumulatedND == null ? 0 : AccumulatedND, AccumulatedO == null ? 0 : AccumulatedO });
                        Lresult.Add(new List<object>() { AccumulatedeD == null ? 0 : AccumulatedeD, AccumulatedeND == null ? 0 : AccumulatedeND, AccumulatedeO == null ? 0 : AccumulatedeO });

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

        private void SetAccumulated(string year, string mes, bool isMedido, List<List<object>> currentNewAccounts)
        {
            string newMes = (int.Parse(mes) - 1).ToString();
            string newYear = year;

            if (mes == "1")
            {
                newMes = "12";
                newYear = (int.Parse(year) - 1).ToString();
            }

            var AccumulatedD = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedND = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "no_domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedO = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "otros" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedeD = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "edomestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedeND = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "eno_domestico" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedeO = _context.AccountsAccumulated.Where(x => x.IsMedido == isMedido && x.type == "eotros" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            newMes = (int.Parse(mes) + 1).ToString();
            newYear = year;


            int totalUsuarios = currentNewAccounts.First().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() == "HA").ToList().Count;
            int totalUsuariose = currentNewAccounts.Last().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() == "HA").ToList().Count;
            if (AccumulatedD != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedD.Accumulated;
                totalUsuariose = totalUsuariose + AccumulatedeD.Accumulated;
            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "domestico" });
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuariose, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "edomestico" });

            totalUsuarios = currentNewAccounts.First().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() == "CO").ToList().Count;
            totalUsuariose = currentNewAccounts.Last().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() == "CO").ToList().Count;
            if (AccumulatedND != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedND.Accumulated;
                totalUsuariose = totalUsuariose + AccumulatedeND.Accumulated;
            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "no_domestico" });
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuariose, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "eno_domestico" });

            totalUsuarios = currentNewAccounts.First().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() != "CO" && JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() != "HA").ToList().Count;
            totalUsuariose = currentNewAccounts.Last().Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() != "CO" && JObject.Parse(JsonConvert.SerializeObject(x))["uso"].ToString() != "HA").ToList().Count;
            if (AccumulatedO != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedO.Accumulated;
                totalUsuariose = totalUsuariose + AccumulatedeO.Accumulated;
            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "otros" });
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuariose, IsMedido = isMedido, Mes = int.Parse(mes), Year = int.Parse(year), type = "eotros" });
            _context.SaveChanges();
        }

        [HttpPost("RunFormato23Desgloce/{isFormato2?}")]
        public async Task<IActionResult> ExectSpFzaFormato23Desgloce([FromRoute] bool isFormato2 = false)
        {
            var body = "";

            object result = null;
            List<object> data = new List<object>();

            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEnd();

                }

                List<SPParameters> parameters = JsonConvert.DeserializeObject<List<SPParameters>>(body);
                parameters.Add(new SPParameters { Key = "error", Size = 200, Direccion = ParameterDirection.InputOutput, DbType = DbType.String, Value = "" });
                result = await new RunSP(this, _context).runProcedureNT("fza_formato2_3_desgloce", parameters, 1);
                data.Add(result);


                string mes = parameters.Where(x => x.Key == "mesPresentarFin").FirstOrDefault()?.Value;
                string year = parameters.Where(x => x.Key == "yearPeriodo").FirstOrDefault()?.Value;
                string newMes = mes;
                string newYear = year;
                if (int.Parse(mes) == 1)
                {
                    newMes = "12";
                    newYear = (int.Parse(year) - 1).ToString();
                }
                else
                {
                    newMes = (int.Parse(mes) - 1).ToString();
                }
                var nextAccumulated = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "actuales" && x.Mes.ToString() == mes && x.Year.ToString() == year).FirstOrDefault()?.Accumulated;

                if (nextAccumulated != null)
                {
                    newMes = mes;
                    newYear = year;
                }

                var AccumulatedD = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "actuales" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                var AccumulatedND = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "anteriores" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;
                var AccumulatedO = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "mixto" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault()?.Accumulated;

                bool create = false;
                if (nextAccumulated == null)
                {

                    if (int.Parse(year) == DateTime.Now.Year && int.Parse(mes) < DateTime.Now.Month)
                    {
                        create = true;
                    }
                    else if (int.Parse(year) < DateTime.Now.Year && int.Parse(mes) == 12 && DateTime.Now.Month == 1)
                    {
                        create = true;
                    }

                    if (create)
                    {
                        SetAccumulatedDesgloce(year, mes, JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(result)));
                    }


                }
                else
                {
                    if (!isFormato2)
                    {
                        data = new List<object>() { new List<object>() };
                    }
                }
                data.Add(new List<object> { AccumulatedD == null ? 0 : AccumulatedD, AccumulatedND == null ? 0 : AccumulatedND, AccumulatedO == null ? 0 : AccumulatedO });
            }
            catch (RunSpException e)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}") });
            }
            catch (Exception e)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}" });
            }

            return StatusCode((int)TypeError.Code.Ok, new { data = data });
        }

        [HttpPost("SetAccumulatedDesgloce/{year}/{mes}")]
        public void SetAccumulatedDesgloce([FromRoute] string year, [FromRoute] string mes, [FromBody] List<object> currentNewAccounts)
        {
            //var nextAccumulated = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "domestactualesico" && x.Mes.ToString() == mes && x.Year.ToString() == year).FirstOrDefault()?.Accumulated;
            //bool create = false;
            //if (nextAccumulated == null)
            //{

            //    if (int.Parse(year) == DateTime.Now.Year && int.Parse(mes) < DateTime.Now.Month)
            //    {
            //        create = true;
            //    }
            //    else if (int.Parse(year) < DateTime.Now.Year && int.Parse(mes) == 12 && DateTime.Now.Month == 1)
            //    {
            //        create = true;
            //    }


            //    //SetAccumulatedDesgloce(year, mes, JsonConvert.DeserializeObject<List<object>> (JsonConvert.SerializeObject(result)));


            //}

            //if (create)
            //{
            string newMes = (int.Parse(mes) - 1).ToString();
            string newYear = year;

            if (mes == "1")
            {
                newMes = "12";
                newYear = (int.Parse(year) - 1).ToString();
            }


            var AccumulatedD = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "actuales" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedND = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "anteriores" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            var AccumulatedO = _context.AccountsAccumulated.Where(x => x.IsMedido == true && x.type == "mixto" && x.Mes.ToString() == newMes && x.Year.ToString() == newYear).FirstOrDefault();

            newMes = (int.Parse(mes) + 1).ToString();
            newYear = year;


            var Jobject = currentNewAccounts.Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["tipo"].ToString() == "actuales").First();
            int totalUsuarios = int.Parse(JObject.Parse(JsonConvert.SerializeObject(Jobject))["usuarios"].ToString());
            if (AccumulatedD != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedD.Accumulated;

            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = true, Mes = int.Parse(mes), Year = int.Parse(year), type = "actuales" });
            Jobject = currentNewAccounts.Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["tipo"].ToString() == "anteriores").First();
            totalUsuarios = int.Parse(JObject.Parse(JsonConvert.SerializeObject(Jobject))["usuarios"].ToString());
            if (AccumulatedND != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedND.Accumulated;

            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = true, Mes = int.Parse(mes), Year = int.Parse(year), type = "anteriores" });
            Jobject = currentNewAccounts.Where(x => JObject.Parse(JsonConvert.SerializeObject(x))["tipo"].ToString() == "mixto").First();
            totalUsuarios = int.Parse(JObject.Parse(JsonConvert.SerializeObject(Jobject))["usuarios"].ToString());
            if (AccumulatedO != null)
            {
                totalUsuarios = totalUsuarios + AccumulatedO.Accumulated;
            }
            _context.AccountsAccumulated.Add(new AccountsAccumulated() { Accumulated = totalUsuarios, IsMedido = true, Mes = int.Parse(mes), Year = int.Parse(year), type = "mixto" });
            _context.SaveChanges();


        }

        [HttpPost("runSpAsignarDeb/{procedure}")]
        public async Task<IActionResult> ExectSpFormatoss([FromRoute] string procedure)
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
                result = await new RunSP(this, _context).runProcedure(procedure, parameters);
                return Ok(result);


            }
            catch (Exception e)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}") });
            }


        }


    }


}