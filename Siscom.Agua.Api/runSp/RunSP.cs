using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Siscom.Agua.Api.Exceptions;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Siscom.Agua.Api.runSp
{

    public class RunSP
    {
        private ControllerBase controller;
        private ApplicationDbContext _context;
        private string error = string.Empty;
        public RunSP(ControllerBase Controllers, ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<object> runProcedure(string nameProcedure, List<SPParameters> parameters)
        {

            List<SPParameters> oOutParameters;
            bool ReturnData = false;
            List<object> data;

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        oOutParameters = new List<SPParameters>();

                        command.CommandText = nameProcedure;
                        command.CommandType = CommandType.StoredProcedure;

                        parameters.ForEach(x =>
                        {

                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@" + x.Key,
                                DbType = x.DbType,
                                Size = x.Size,
                                Value = x.Value,
                                Direction = x.Direccion
                            });
                            if (x.Direccion == ParameterDirection.InputOutput || x.Direccion == ParameterDirection.Output)
                                oOutParameters.Add(x);
                        });

                        this._context.Database.OpenConnection();
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            IDataReader odata = result;
                            data = new List<object>();
                            if (result.HasRows)
                            {

                                error = command.Parameters["@error"].Value.ToString();

                                while (odata.Read())
                                {

                                    string jData = "{";
                                    for (int index = 0; index < odata.FieldCount; index++)
                                    {
                                        jData += "'" + odata.GetName(index) + "':'" + odata.GetValue(index).ToString() + "'";
                                        if (index + 1 < odata.FieldCount)
                                        {
                                            jData += ",";
                                        }

                                    }
                                    jData += "}";
                                    data.Add(JObject.Parse(jData));
                                }


                            }

                            if (string.IsNullOrEmpty(error))
                            {
                                oOutParameters.ForEach(x =>
                                {
                                    x.Value = command.Parameters["@" + x.Key].Value.ToString();
                                });

                                scope.Complete();

                            }
                            else
                            {
                                throw new RunSpException($"Error: [{error}]");

                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                //systemLog.Controller = controller.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = controller.ControllerContext.RouteData.Values["action"].ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                throw new Exception("Problemas para ejecutar la transacción");
                //return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });

            }
            return new { paramsOut = oOutParameters, data = data };
        }


        public async Task<object> runProcedureNT(string nameProcedure, List<SPParameters> parameters, int ISdatatable = 0)
        {

            List<SPParameters> oOutParameters;
            bool ReturnData = false;
            List<object> data;

            try
            {

                var dataTable = new DataTable();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    oOutParameters = new List<SPParameters>();

                    command.CommandText = nameProcedure;
                    command.CommandType = CommandType.StoredProcedure;

                    parameters.ForEach(x =>
                    {

                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@" + x.Key,
                            DbType = x.DbType,
                            Size = x.Size,
                            Value = x.Value,
                            Direction = x.Direccion
                        });
                        if (x.Direccion == ParameterDirection.InputOutput || x.Direccion == ParameterDirection.Output)
                            oOutParameters.Add(x);
                    });

                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        IDataReader odata = result;
                        data = new List<object>();
                        if (ISdatatable == 1)
                        {
                            dataTable.Load(result);
                            return dataTable;
                        }
                        if (result.HasRows)
                        {

                            error = command.Parameters["@error"].Value.ToString();

                            while (odata.Read())
                            {

                                string jData = "{";
                                for (int index = 0; index < odata.FieldCount; index++)
                                {
                                    jData += "'" + odata.GetName(index) + "':'" + odata.GetValue(index).ToString() + "'";
                                    if (index + 1 < odata.FieldCount)
                                    {
                                        jData += ",";
                                    }

                                }
                                jData += "}";
                                data.Add(JObject.Parse(jData));
                            }


                        }

                        if (string.IsNullOrEmpty(error))
                        {
                            oOutParameters.ForEach(x =>
                            {
                                x.Value = command.Parameters["@" + x.Key].Value.ToString();
                            });



                        }
                        else
                        {
                            throw new RunSpException($"Error: [{error}]");

                        }

                    }

                }

            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = controller.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = controller.ControllerContext.RouteData.Values["action"].ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                throw new Exception("Problemas para ejecutar la transacción");
                //return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });

            }
            return new { paramsOut = oOutParameters, data = data };
        }
    }
}
