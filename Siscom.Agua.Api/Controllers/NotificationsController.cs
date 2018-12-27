using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Siscom.Agua.Api.Services.FirebaseService;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendNotification()
        {
            // Instanciating with base URL  
            FirebaseDB firebaseDB = new FirebaseDB("https://siscom-notifications.firebaseio.com/");

            // Referring to Node with name "Notifications"  
            FirebaseDB firebaseDBNotifications = firebaseDB.Node("Notifications");
            var JsonConverter = JsonConvert.SerializeObject(new MessagesNotification
            {
                Title = "Cancelación",
                Message = "Se requiere cancelación"
            });

            FirebaseResponse postResponse = firebaseDBNotifications.Post(JsonConverter);

            return Ok();
        }
    }

    public class MessagesNotification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }

    public class DetailAjusment
    {
        public int AgreementId { get; set; }
        public int DebtId { get; set; }
        public int Porcent { get; set; }
        public string TextDiscount { get; set; }
    }
}