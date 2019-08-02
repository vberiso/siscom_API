﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Exceptions;
using Siscom.Agua.Api.runSp;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;


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
        public async Task<IActionResult> ExectSpBillingPeriod([FromRoute] int id_agreement, [FromRoute] int start_month, [FromRoute] int end_month, [FromRoute] int from_year, [FromRoute] int simulate)
        {
            int mesesEfectuados = 0;
            object result= null;
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
                    result = await new RunSP(this, _context).runProcedure("accrual_period", parameters);
                    parameters = null;
                    mesesEfectuados++;
                }
                catch (RunSpException e)
                {
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"{e.ToMessageAndCompleteStacktrace()}"), Message = $"Se regiostrarón los {mesesEfectuados} primeros meses"  });
                }
                catch (Exception e)
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = $"{e.ToMessageAndCompleteStacktrace()}", Message = $"Se regiostrarón los {mesesEfectuados} primeros meses" });
                }
            }
            return StatusCode((int)TypeError.Code.Ok, new { Message = $"Se registrarón todos los meses correctamente", data= result });
        }



    }

  

    

}