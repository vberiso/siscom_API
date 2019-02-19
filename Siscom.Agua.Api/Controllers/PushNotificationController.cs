using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Siscom.Agua.Api.Services.FirebaseService;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    //[Authorize]
    public class PushNotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PushNotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] PushNotifications notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Instanciating with base URL  
            FirebaseDB firebaseDB = new FirebaseDB("https://siscom-notifications.firebaseio.com/");

            // Referring to Node with name "Notifications"  
            FirebaseDB firebaseDBNotificationsCancelation = firebaseDB.Node("Notifications");
            FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("Discount");
           
            await _context.PushNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            var JsonConverter = JsonConvert.SerializeObject(notification);
            if (notification.Type == "NOT01")
            {
                FirebaseResponse postResponse = firebaseDBNotificationsCancelation.Post(JsonConverter);
            }
            else if (notification.Type == "NOT02")
            {
                FirebaseResponse getResponse = firebaseDBNotificationsDiscount.Post(JsonConverter);
            }
           

            
           

            return Ok();
        }

        //[HttpPost]
        //public async Task<IActionResult> SendNotification()
        //{
        //    // Instanciating with base URL  
        //    FirebaseDB firebaseDB = new FirebaseDB("https://siscom-notifications.firebaseio.com/");

        //    // Referring to Node with name "Notifications"  
        //    FirebaseDB firebaseDBNotificationsCancelation = firebaseDB.Node("Notifications");
        //    FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("Discount");
        //    //if(notification.Type == "NOT01")
        //    //{
        //    //    await _context.PushNotifications.AddAsync(notification);
        //    //    await _context.SaveChangesAsync();
        //    //}
        //    //else if (notification.Type == "NOT02")
        //    //{

        //    //}
        //    //notification.Reason = "se solicita descuento"
        //    var JsonConverter = JsonConvert.SerializeObject(new MessagesNotification
        //    {
        //        Title = "Descuento",
        //        Message = "Generar Descuento",
        //        IsActive = false
        //    });

        //    FirebaseResponse postResponse = firebaseDBNotificationsCancelation.Post(JsonConverter);
        //    //FirebaseResponse getResponse = firebaseDBNotificationsDiscount.Post(JsonConverter);

        //    return Ok();
        //}
    }

}

public class MessagesNotification
{
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsActive { get; set; }
}

    //public class DetailAjusment
    //{
    //    public int AgreementId { get; set; }
    //    public int DebtId { get; set; }
    //    public int Porcent { get; set; }
    //    public string TextDiscount { get; set; }
    //}
//}