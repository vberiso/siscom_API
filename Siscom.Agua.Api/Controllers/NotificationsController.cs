using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Siscom.Agua.Api.Services.FirebaseService;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] PushNotifications notification)
        {
            // Instanciating with base URL  
            FirebaseDB firebaseDB = new FirebaseDB("https://siscom-notifications.firebaseio.com/");

            // Referring to Node with name "Notifications"  
            FirebaseDB firebaseDBNotificationsCancelation = firebaseDB.Node("Notifications");
            FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("Discount");
            if(notification.Type == "NOT01")
            {

            }
            else if (notification.Type == "NOT02")
            {

            }
            var JsonConverter = JsonConvert.SerializeObject(notification);

            FirebaseResponse postResponse = firebaseDBNotificationsCancelation.Post(JsonConverter);
            FirebaseResponse getResponse = firebaseDBNotificationsDiscount.Post(JsonConverter);

            return Ok();
        }
    }

    //public class MessagesNotification
    //{
    //    public string Title { get; set; }
    //    public string Message { get; set; }
    //    public bool IsActive { get; set; }
    //}

    //public class DetailAjusment
    //{
    //    public int AgreementId { get; set; }
    //    public int DebtId { get; set; }
    //    public int Porcent { get; set; }
    //    public string TextDiscount { get; set; }
    //}
}