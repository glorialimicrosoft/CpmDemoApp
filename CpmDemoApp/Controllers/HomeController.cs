using Azure.Communication.Messages;
using CpmDemoApp.Models;
using CpmDemoApp.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CpmDemoApp.Controllers
{
    public class HomeController : Controller
    {
        private static bool _clientInitialized;
        private static NotificationMessagesClient _notificationMessagesClient;
        private static Guid _channelRegistrationId;

        public HomeController(IOptions<ClientOptions> options)
        {
            if (!_clientInitialized)
            {
                _channelRegistrationId = Guid.Parse(options.Value.ChannelRegistrationId);
                _notificationMessagesClient = new NotificationMessagesClient(options.Value.ConnectionString);
                _clientInitialized = true;
            }
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public JsonResult GetNewMessages()
        {
            var newMessages = Data.NewIncomingMessagesListStatic; // Assuming this is your data source
            return Json(newMessages);
        }
        public IActionResult NewIncomingMessages()
        {
            return PartialView("NewMessages");
        }


    }
}