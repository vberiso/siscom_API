using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
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

        
        [HttpGet("GetNotificationByIdsAgreement/{ids}")]
        public async Task<IActionResult> GetNotificationByIdsAgreement([FromRoute] string ids)
        {
            var datatable = new DataTable();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAgreementByIdAgreements]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter { ParameterName = "@AgreemenIds", DbType = DbType.String, Value = ids });
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

        [HttpGet("GetFiles/{TypeFile?}")]
        public async Task<IActionResult> GetFiles([FromRoute] string TypeFile = "FI001")
        {
            var archivos = await _context.NotificationFiles.Select(x => new FileNotifications
            {
                Id = x.Id,
                FileName = x.FileName,
                GenerationDate = x.GenerationDate,
                UserName = x.UserName,
                UserId = x.UserId,
                Folio = x.Folio,
                TotalRecords = x.TotalRecords,
                TypeFile = x.TypeFile
            }).Where(x => x.TypeFile == TypeFile).OrderByDescending(x => x.GenerationDate).ToListAsync();
            return Ok(archivos);
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
                PDFNotifications = x.PDFNotifications,
                Folio = x.Folio
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

        [HttpPost("ayuntamiento")]
        public async Task<IActionResult> PostNotificationsAyuntamiento([FromBody] string Agreements)
        {
            List<string> agreements = new List<string>(Agreements.Split(","));
            List<object> LAgremments = new List<object>();
            int idNotification = 0;
            List<string> error = new List<string>();
            try
            {
                List<string> statusDeuda = new List<string>() { "ED001", "ED004", "ED007", "ED011" };
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
                    if (idNotification != 0)
                    {
                        var notifi = await _context.Notifications.Where(x => x.Id == idNotification).FirstOrDefaultAsync();

                        var agre = await _context.Agreements
                            .Include(x => x.AgreementDetails)
                            .Include(x => x.TypeIntake)
                              .Include(x => x.Debts)
                                    .ThenInclude(x => x.DebtDetails)
                                 .Include(x => x.Addresses)
                                        .ThenInclude(x => x.Suburbs)
                                            .ThenInclude(x => x.Towns)
                                                        .ThenInclude(x => x.States)
                                .Include(x => x.Clients).Where(x => x.Id == int.Parse(item)).FirstOrDefaultAsync();
                        agre.Debts = agre.Debts.Where(d => statusDeuda.Contains(d.Status)).ToList();
                        agre.Clients = agre.Clients.Where(c => c.TypeUser == "CLI01").ToList();
                        agre.Addresses = agre.Addresses.Where(a => a.TypeAddress == "DIR01").ToList();

                        LAgremments.Add(new { agreement = agre, notifi,

                            idNotification });
                       
                    }

                    idNotification = 0;
                }

                if (error.Count > 0)
                {
                    if (agreements.Count != error.Count)
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
                    return Ok(LAgremments);
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
        [HttpPost("UPdatePDF/{idFile?}")]
        public async Task<IActionResult> UPdatePDF(IFormFile AttachedFile, [FromRoute] string idFile )
        {
            var file = _context.NotificationFiles.Find(int.Parse(idFile));
            if (AttachedFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {

                    await AttachedFile.CopyToAsync(memoryStream);
                    file.PDFNotifications = memoryStream.ToArray();
                }
                _context.SaveChanges();
            }
            return Ok();
        }
        [HttpPost("AddFileResult/{TypeFile?}/{TotalRecords?}")]
        public async Task<IActionResult> addPDF(IFormFile AttachedFile, [FromRoute] string TypeFile = "FI001", [FromRoute] int TotalRecords = 0)
        {
            FileNotifications NotificationsData = null;
            NotificationFiles file =null;
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    NotificationsData = JsonConvert.DeserializeObject<FileNotifications>(Request.Form["Data"].ToString());
                    file = new NotificationFiles();
                    file.UserId = NotificationsData.UserId;
                    file.UserName = NotificationsData.UserName;
                    file.GenerationDate = NotificationsData.GenerationDate;
                    file.FileName = NotificationsData.FileName;
                 
                    file.TypeFile = TypeFile;
                    file.TotalRecords = TotalRecords;

                    if (AttachedFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {

                            await AttachedFile.CopyToAsync(memoryStream);
                            file.PDFNotifications = memoryStream.ToArray();
                        }
                    }

                    await _context.NotificationFiles.AddAsync(file);
                    await _context.SaveChangesAsync();
                    if (NotificationsData.notifications != null) {
                        NotificationsData.notifications.ToList().ForEach(x =>
                        {
                            var notif = _context.Notifications.Find(x.Id);
                            notif.NotificationFiles = file.Id;
                            _context.Entry(notif).State = EntityState.Modified;
                            _context.SaveChanges();

                        });
                    }
                    else if(NotificationsData.IdNotification != null)
                    {
                        var Idnotifications = NotificationsData.IdNotification.Split(",").ToList();
                        if (Idnotifications.Count >0) {
                            Idnotifications.ForEach(x =>
                            {
                                var notif = _context.Notifications.Where(n => n.Id.ToString() == x).FirstOrDefault();
                                if (notif != null)
                                {
                                    notif.NotificationFiles = file.Id;
                                    _context.Entry(notif).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }

                            });
                        }
                    }
                    scope.Complete();
                }
               
                return Ok(file.Id);
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

        [HttpPost("getFilesAccountStatus")]
        public async Task<IActionResult> getFilesAccountStatuss([FromBody] int idAgreements = 0)
        {
            List<string> statusDeuda = new List<string>() { "ED001", "ED004", "ED007", "ED011" };
            IQueryable<Siscom.Agua.DAL.Models.AccountStatusInFile> query = _context.AccountStatusInFiles;
            List<object> Agrements = new List<object>();
            //if (idAgreements.Count >0)
            //{
            //    query = query.Where(x => idAgreements.Contains(x.Id));

            //}
            //var files = query.ToList();
            try
            {
                //files.ForEach(x =>
                //{
                //    var agree = _context.Agreements
                //                        .Include(a => a.Addresses)
                //                            .ThenInclude(s => s.Suburbs)
                //                                .ThenInclude(t => t.Towns)
                //                                    .ThenInclude(st => st.States)
                //                        .Include(c => c.Clients)
                //                        .Include(ad =>ad.AccountStatusInFiles)
                //                        .Include(d => d.Debts)
                //                         .ThenInclude(dd => dd.DebtDetails)
                //                        .Include(ti => ti.TypeIntake)
                //                        .Include(ts => ts.TypeStateService)
                //                        .Include(sd => sd.TypeService)
                //                        .Include(tc => tc.TypeConsume)
                //                        .Include(ad => ad.AgreementDetails)
                //                        .Include(di => di.AgreementDiscounts)
                //                            .ThenInclude(d => d.Discount)
                //    .Where(a => a.Id == x.AgreementId).First();
                //    agree.AccountStatusInFiles = new List<AccountStatusInFile>() { x };
                //    agree.Debts = agree.Debts.Where(d => statusDeuda.Contains(d.Status)).ToList();
                //    Agrements.Add(new  { agreement = JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(agree,new JsonSerializerSettings
                //    {
                //        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                //        Formatting = Formatting.Indented,
                //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //    })) }) ;
                //});

                var file = _context.AccountStatusInFiles.Find(idAgreements);

                return Ok(file);
            }
            catch (Exception e)
            {
                return null;
            }

            //return Ok(Agrements);
        }
        [HttpPost("addFilesAccountStatus")]
        public async Task<IActionResult> addFilesAccountStatus([FromBody] object dataP )
        {
            List<int> files = null;
            NotificationFiles file = null;
            try
            {
                var OData = JObject.Parse(JsonConvert.SerializeObject(dataP));
                List<int> idAgreements = JsonConvert.DeserializeObject<List<int>>(JsonConvert.SerializeObject(OData["idAgreements"]));
                FileNotifications data = JsonConvert.DeserializeObject<FileNotifications>(JsonConvert.SerializeObject(OData["Data"]));
                files = new List<int>();
                //var data = JsonConvert.DeserializeObject<FileNotifications>(Request.Form["Data"].ToString());
                
                    var Afile = new AccountStatusInFile() {
                        AgreementId = idAgreements.First(),
                        FileName = data.FileName,
                        GenerationDate = data.GenerationDate,
                        UserId = data.UserId,
                        PDFBytes = data.PDFNotifications,
                        UserName = data.UserName

                    };
                    _context.AccountStatusInFiles.Add(Afile);
                     _context.SaveChanges();
             //       files.Add( Afile.Id);
             
                return Ok(Afile.Id);
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(JsonConvert.SerializeObject(files));
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para subir el archivo de las notificaciones" });
            }
        }


        #endregion
    }
}