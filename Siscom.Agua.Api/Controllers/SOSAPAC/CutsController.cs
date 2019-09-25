using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model.SOSAPAC;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    public class CutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CutsController(ApplicationDbContext context)
        {

            _context = context;
        }
        // GET: api/Suburbs
        [HttpGet("Suburbs")]
        public async Task<IActionResult> GetSuburbs()
        {
            //var dataTable = new System.Data.DataTable();

            //using (var connection = _context.Database.GetDbConnection())
            //{
            //    await connection.OpenAsync();
            //    using (var command = connection.CreateCommand())
            //    {
            //        command.CommandText = "SELECT * FROM [dbo].[GetSuburbs]";
            //        using (var result = await command.ExecuteReaderAsync())
            //        {
            //            dataTable.Load(result);
            //        }
            //    }
            //}
            //var data = ConvertDataTable<CutSuburbVM>(dataTable);
            var data = await _context.Routes.ToListAsync();
            return Ok(data);

        }

        // GET: api/Suburbs/2/5000/
        [HttpGet("{NumOfPeriods}/{Debit}/{Suburbs}")]
        public async Task<IActionResult> GetSuburbs([FromRoute] int NumOfPeriods, [FromRoute] decimal Debit, [FromRoute] string Suburbs)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementBySuburbs]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Suburbs",
                        DbType = DbType.String,
                        Value = Suburbs
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@NumPeriods",
                        DbType = DbType.Int32,
                        Value = NumOfPeriods
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Amount",
                        DbType = DbType.Decimal,
                        Value = Debit
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(result);
                    }
                }
                if (dataTable.Rows.Count <= 0)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se encontraron datos con los parametros especificados" });
                }
                else
                {
                    return Ok(dataTable);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            
        }

        [HttpGet("GetNotificationByAccount/{account}")]
        public async Task<IActionResult> GetNotificationByAccount([FromRoute] string account)
        {
            var datatable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementByAccount]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter { ParameterName = "@Account", DbType = DbType.String, Value = account });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        datatable.Load(result);
                    }
                }
                if (datatable.Rows.Count <= 0)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "El contribuyente de esta cuenta va al corriente de sus pagos o está en estado cortado, por lo cual no puede mandar una orden de corte" });
                }
                else
                {
                    return Ok(datatable);
                }
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }

        [HttpGet("GetFiles")]
        public async Task<IActionResult> GetFiles()
        {
            return Ok(await _context.NotificationFiles.Select(x => new FileNotifications
            {
                Id = x.Id,
                FileName = x.FileName,
                GenerationDate = x.GenerationDate,
                UserName = x.UserName,
                UserId = x.UserId,
            }).OrderByDescending(x => x.GenerationDate).ToListAsync());
        }


        [HttpGet("GetFilesById/{Id}")]
        public async Task<IActionResult> GetFiles([FromRoute] int Id)
        {
            return Ok(await _context.NotificationFiles.Where(x => x.Id == Id).Select(x => new FileNotifications
            {
                Id = x.Id,
                FileName = x.FileName,
                GenerationDate = x.GenerationDate,
                UserName = x.UserName,
                UserId = x.UserId,
                PDFNotifications = x.PDFNotifications
            }).FirstOrDefaultAsync());
        }

        [HttpPost()]
        public async Task<IActionResult> PostNotifications([FromBody] string Agreements)
        {
            List<string> agreements = new List<string>(Agreements.Split(","));
            List<NotificationVM> Notification = new List<NotificationVM>();
            int idNotification = 0;
            List<string> error = new List<string>();
            try
            {
                foreach (var item in Agreements.Split(","))
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "[dbo].[generate_notification]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@id_agreement",
                            DbType = DbType.Int32,
                            Value = Convert.ToInt32(item)
                        });

                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@error",
                            DbType = DbType.String,
                            Size = 200,
                            Direction = ParameterDirection.Output
                        });
                        //@IdNotification
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@IdNotification",
                            DbType = DbType.Int32,
                            Direction = ParameterDirection.Output
                        });

                        this._context.Database.OpenConnection();
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            if (!string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()))
                                error.Add("El contrado (ID: " + item + ") -> " + command.Parameters["@error"].Value.ToString());
                            else
                                idNotification = Convert.ToInt32(command.Parameters["@IdNotification"].Value.ToString());
                        }
                    }
                    if(idNotification != 0)
                    {
                        Notification.Add(await _context.Notifications.Select(x => new NotificationVM
                        {
                            Id = x.Id,
                            AgreementId = x.AgreementId,
                            Folio = x.Folio,
                            FromDate = x.FromDate,
                            NotificationDate = x.NotificationDate,
                            Rounding = x.Rounding,
                            Status = x.Status,
                            Subtotal = x.Subtotal,
                            Tax = x.Tax,
                            Total = x.Total,
                            UntilDate = x.UntilDate
                        }).Where(x => x.Id == idNotification).FirstOrDefaultAsync());
                    }
                    
                    idNotification = 0;
                }
                
                if(error.Count > 0)
                {
                    if(agreements.Count != error.Count)
                    {
                        return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format($"Se completaron {agreements.Count - error.Count} de {agreements.Count} de el proceso de notificación pero no se genero notificacion para las siguientes cuentas ID: [{string.Join(Environment.NewLine, error)}]") });
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format($"No se pudo realizar el proceso de descuentos por las siguientes razones: [{string.Join(Environment.NewLine, error)}]") });
                    }

                }
                else
                {
                    return Ok(Notification);
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = $"Agreement List: {Agreements}";
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción de notificaciones" });
            }
           
        }

        [HttpPost("AddFileResult")]
        public async Task<IActionResult> addPDF(IFormFile AttachedFile)
        {
            FileNotifications NotificationsData = null;
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    NotificationsData = JsonConvert.DeserializeObject<FileNotifications>(Request.Form["Data"].ToString());
                    NotificationFiles file = new NotificationFiles();
                    file.UserId = NotificationsData.UserId;
                    file.UserName = NotificationsData.UserName;
                    file.GenerationDate = NotificationsData.GenerationDate;
                    file.FileName = NotificationsData.FileName;

                    using (var memoryStream = new MemoryStream())
                    {
                        await AttachedFile.CopyToAsync(memoryStream);
                        file.PDFNotifications = memoryStream.ToArray();
                    }

                    await _context.NotificationFiles.AddAsync(file);
                    await _context.SaveChangesAsync();

                    NotificationsData.notifications.ToList().ForEach(x =>
                    {
                        var notif = _context.Notifications.Find(x.Id);
                        notif.NotificationFiles = file.Id;
                        _context.Entry(notif).State = EntityState.Modified;
                        _context.SaveChanges();

                    });
                    scope.Complete();
                }
                return Ok();
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(JsonConvert.SerializeObject(NotificationsData));
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para subir el archivo de las notificaciones" });
            }
        }

        [HttpPost("WithNotifications")]
        public async Task<IActionResult> GetAgreementsWithNotifications([FromBody] SendCutsVM send)
        {
            string error = string.Empty;
            var dataTable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementByNotifications]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Suburbs",
                        DbType = DbType.String,
                        Value = send.Routes
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@NumPeriods",
                        DbType = DbType.Int32,
                        Value = send.Periods
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Amount",
                        DbType = DbType.Decimal,
                        Value = send.Amount
                    });

                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@NumNotifications",
                        DbType = DbType.Int32,
                        Value = send.NumNotification
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(result);
                    }
                }
                if (dataTable.Rows.Count <= 0)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se encontraron datos con los parametros especificados." });
                }
                else
                {
                    return Ok(dataTable);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        #region Private Methods
        private List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private T GetItem<T>(DataRow dr)
        {
            System.Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        try
                        {
                            var convertedValue = GetValueByDataType(pro.PropertyType, dr[column.ColumnName]);
                            pro.SetValue(obj, convertedValue, null);
                        }
                        catch (Exception e)
                        {
                            //ex handle code                   
                            throw;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        private object GetValueByDataType(System.Type propertyType, object o)
        {
            if (o.ToString() == "null")
            {
                return null;
            }
            if (propertyType == (typeof(Guid)) || propertyType == typeof(Guid?))
            {
                return Guid.Parse(o.ToString());
            }
            else if (propertyType == typeof(int) || propertyType.IsEnum)
            {
                return Convert.ToInt32(o);
            }
            else if (propertyType == typeof(decimal))
            {
                return Convert.ToDecimal(o);
            }
            else if (propertyType == typeof(long))
            {
                return Convert.ToInt64(o);
            }
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return Convert.ToBoolean(o);
            }
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                return Convert.ToDateTime(o);
            }
            return o.ToString();
        }
        private static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        #endregion
    }
}