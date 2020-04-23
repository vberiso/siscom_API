using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.Providers;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using Swashbuckle.AspNetCore.Examples;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DispatchOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings appSettings;

        public DispatchOrderController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.appSettings = appSettings.Value;
        }

        [HttpGet("DispatchOrderById/{id}")]
        public async Task<IActionResult> getAgremmentsOfStaff([FromRoute] int id)
        {
            var dispatchOrder =  await _context.DispatchOrders.FirstOrDefaultAsync(x => x.Id == id);
            return StatusCode(StatusCodes.Status200OK, dispatchOrder);
        }

        [HttpGet("DispatchOrderByUserId/{userId}")]
        public async Task<IActionResult> getAgremmentsOfStaffByUserId([FromRoute] String userId)
        {
            var dispatchOrder = new List<DispatchOrder>();
            dispatchOrder = _context.DispatchOrders.Where(x => x.UserId == userId && x.Status == "DSO01").ToList();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dispatchOrder.ForEach(x =>
                    {
                        var entry = _context.DispatchOrders.Find(x.Id);
                        x.Status = "DSO02";
                        x.DateSynchronized = DateTime.Now;
                        _context.Entry(entry).CurrentValues.SetValues(x);
                        _context.SaveChanges();
                    });
                    scope.Complete();
                }
                    
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject("");
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualzar el despacho de ordenes" });

            }

            return StatusCode(StatusCodes.Status200OK, dispatchOrder);
        }


        [HttpPost()]
        public async Task<IActionResult> PostDispatchOrder([FromBody] DispatchOrder dispatchOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {   
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dispatchOrder1 = await _context.DispatchOrders.Where(d => d.OrderWorkId == dispatchOrder.OrderWorkId).FirstOrDefaultAsync();

                    if(dispatchOrder1 == null)
                    {
                        _context.DispatchOrders.Add(dispatchOrder);
                        await _context.SaveChangesAsync();
                        scope.Complete();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK, new { msg = "Ya existe un despacho ligado a esta order work" });
                    }                    
                }
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(dispatchOrder);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode(StatusCodes.Status400BadRequest , new { Error = "Problemas para agregar un dispatch_order" });
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> PutDispatchOrder([FromRoute] int id, [FromBody] DispatchOrder dispatchOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != dispatchOrder.Id)
                    {
                        return BadRequest();
                    }
                    _context.Entry(dispatchOrder).State = EntityState.Modified;                    
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
                return StatusCode(StatusCodes.Status200OK, new { msg = "Actualización exitosa" }  );
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(dispatchOrder);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar un dispatch_order" });
            }
        }

        /// <summary>
        /// Send data from app mobile
        /// </summary>
        /// <param name="syncData"></param>
        /// <returns></returns>
        /// <remarks>
        /// Date Fields example 2020-04-09 12:39:15
        /// </remarks>
        [HttpPost("SyncDataMobile")]
        [SwaggerRequestExample(typeof(SyncDataMobileVM), typeof(SyncDataMobileExampleProvider))]
        public async Task<IActionResult> SyncData(SyncDataMobileVM syncData)
        {
            if (ModelState.IsValid)
            {
                string errorSP = string.Empty;
                bool validDateOrderWork = true;
                bool validDatePhoto = true;
                string exceptionMessage = string.Empty;
                var uploadFilesPath = Path.Combine(appSettings.FilePath, "FotosOT", DateTime.Now.ToString("yyyy-MM-dd"), syncData.UserIdAPI);
                DispatchOrder dispatch = await _context.DispatchOrders.FindAsync(syncData.IdDispatchOrder);
                OrderWork order = await _context.OrderWorks.FindAsync(dispatch.OrderWorkId);
                var user = await _context.Users.FindAsync(syncData.UserIdAPI);
                switch (syncData.StatusOrderWork){
                    case "FINALIZADA":
                        order.Status = "EOT03";
                        break;
                    case "NO EJECUTADA":
                        order.Status = "EOT04";
                        var coment = await _context.ReasonCatalog.Where(x => x.Id == syncData.idReasonCatalog).FirstOrDefaultAsync();
                        order.ObservationMobile = coment.Description;
                        await _context.OrderWorkReasonCatalog.AddAsync(new OrderWorkReasonCatalog
                        {
                            OrderWork = order,
                            ReasonCatalog = coment
                        });
                        await _context.SaveChangesAsync();
                        break;

                }

                DateTime dateRealization;
                DateTime.TryParse(syncData.DateRealization, out dateRealization);
                if (dateRealization == default)
                {
                    return Conflict(new { error = "Fecha con mal formato favor de verificar" });
                }
                order.DateRealization = dateRealization;
                dispatch.DateAttended = dateRealization;

                foreach (var item in syncData.OrderWorkStatuses)
                {
                    DateTime valid;
                    DateTime.TryParse(item.DateOrderWorkStatus, out valid);
                    if (valid == default)
                    {
                        validDateOrderWork = false;
                        break;
                    }
                    else
                    {
                        _context.OrderWorkStatus.Add(new OrderWorkStatus
                        {
                            OrderWork = order,
                            OrderWorkId = order.Id,
                            IdStatus = item.IdStatus,
                            OrderWorkStatusDate = valid,
                            User = string.Format("{0} {1} {2}", user.Name, user.LastName, user.SecondLastName)
                        });
                        _context.SaveChanges();
                    }

                }
                if (!validDateOrderWork)
                {
                    return Conflict(new { error = "Fecha con mal formato dentro de OrderWorkStatus favor de verificar" } );
                }

                foreach (var item in syncData.PhotoSyncMobiles)
                {
                    DateTime valid;
                    DateTime.TryParse(item.DateTake, out valid);
                    if (valid == default)
                    {
                        validDatePhoto = false;
                        break;
                    }
                    else
                    {
                        FileInfo fi = null;
                        //Check if directory exist
                        if (!System.IO.Directory.Exists(uploadFilesPath))
                        {
                            System.IO.Directory.CreateDirectory(uploadFilesPath); //Create directory if it doesn't exist
                        }
                        Guid guid = Guid.NewGuid();
                        string imageName = guid.ToString() + ".jpg";
                        string imgPath = Path.Combine(uploadFilesPath, imageName);
                        byte[] imageBytes = Convert.FromBase64String(item.Photo);
                        try
                        {
                            await System.IO.File.WriteAllBytesAsync(imgPath, imageBytes);
                            fi = new FileInfo(imgPath);
                            var fileSize = FileConverterSize.SizeSuffix(fi.Length);
                            PhotosOrderWork photosOrder = new PhotosOrderWork
                            {
                                BlobPhoto = imageBytes,
                                DatePhoto = valid,
                                NameFile = imageName,
                                OrderWork = order,
                                OrderWorkId = order.Id,
                                PathFile = imgPath,
                                Size = fi.Length,
                                Type = "OTF01",
                                Weight = Math.Round(Convert.ToDouble(fileSize.Split(' ')[0])) + " " + fileSize.Split(' ')[1],
                            User = syncData.UserIdAPI,
                                UserName = string.Format("{0} {1} {2}", user.Name, user.LastName, user.SecondLastName)
                            };
                            _context.PhotosOrderWork.Add(photosOrder);
                            _context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            exceptionMessage = e.Message;
                        }
                        
                    }
                }

                if (!validDatePhoto)
                {
                    return Conflict(new { error = "Fecha con mal formato dentro de lista de fotos, favor de verificar" });
                }

                if (!string.IsNullOrEmpty(exceptionMessage))
                {
                    return Conflict(new { error = "Fecha con mal formato dentro de lista de fotos, favor de verificar" });
                }

                //Update Order
                order.ObservationMobile = string.Format("{0} @ {1}", syncData.OpeningCommentary, syncData.FinalCommentary); 
                _context.Entry(order).State = EntityState.Modified;
                _context.SaveChanges();
                //Update DispatchOrder
                _context.Entry(dispatch).State = EntityState.Modified;
                _context.SaveChanges();

                foreach (var item in syncData.LocationSyncMobiles)
                {
                    LocationOfAttentionOrderWork location = new LocationOfAttentionOrderWork
                    {
                        Latitude = item.Latitud,
                        Longitude = item.Longitud,
                        Type = item.Type == "INICIO" ? "LOW01" : "LOW02"
                    };

                    _context.LocationOfAttentionOrderWorks.Add(location);

                    _context.LocationOrderWorks.Add(new LocationOrderWork
                    {
                        OrderWork = order,
                        LocationOfAttentionOrderWork = location
                    });

                    _context.SaveChanges();
                }


                if (order.Status == "EOT03")
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter() { ParameterName = "@id", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = order.AgrementId },
                        new SqlParameter() { ParameterName = "@isAgreement", SqlDbType = System.Data.SqlDbType.Bit, Direction = System.Data.ParameterDirection.Input, Value = 1 },
                        new SqlParameter() { ParameterName = "@idProduct", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = syncData.IdReconectionCost },
                        new SqlParameter() { ParameterName = "@Observations", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = syncData.OpeningCommentary + ", " + syncData.FinalCommentary, Size = 800 },
                        new SqlParameter() { ParameterName = "@UserId", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = syncData.UserIdAPI, Size = 450 },
                        new SqlParameter() { ParameterName = "@UserName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = string.Format("{0} {1} {2}", user.Name, user.LastName, user.SecondLastName), Size = 256 },
                        new SqlParameter() { ParameterName = "@TypeOrder", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = order.Type, Size = 5 },
                        new SqlParameter() { ParameterName = "@error", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 200 },
                    };

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[OrderWork_Update]";
                        command.Parameters.AddRange(parameters);
                        await _context.Database.OpenConnectionAsync();
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            errorSP = command.Parameters["@error"].Value.ToString();
                        }
                    }
                }
                if (string.IsNullOrEmpty(errorSP))
                    return Ok();
                else
                    return Conflict(new { error = "Error al ejecutar la afectación al cobro [sp]" });
            }
            return BadRequest();
        }

        

    }
}