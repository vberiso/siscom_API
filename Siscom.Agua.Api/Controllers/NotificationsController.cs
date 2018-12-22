using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Siscom.Agua.Api.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class NotificationsController : ControllerBase
    //{
    //    public async Task<IActionResult> SendNotification()
    //    {
    //        IFirebaseConfig config = new FirebaseConfig
    //        {
    //            AuthSecret = "AIzaSyBVvhp66KjzBwhC_uI9F_YK7vJ6AicIxkY",
    //            BasePath = "https://siscom-notifications.firebaseio.com"
    //        };
    //        IFirebaseClient client = new FirebaseClient(config);
    //        var message = new MessagesNotification
    //        {
    //            Title = "Titulo 1",
    //            Message = "Mensaje de prueba"
    //        };

    //        PushResponse response = await client.PushAsync("Cancelacion/push", message);
    //        //response.Result.name;
    //        return Ok();
    //    }
    //}

    //public class MessagesNotification
    //{
    //    public string Title { get; set; }
    //    public string Message { get; set; }
    //}
}