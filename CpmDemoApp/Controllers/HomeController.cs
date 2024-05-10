using Azure.Communication.Messages;
using CpmDemoApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Azure;
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
            return View();
        }

        [HttpPost]
        public IActionResult MessagesList()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult ClearHistory()
        {
            EventsReceived.EventReceivedListStatic = new List<EventReceived>();
            return RedirectToAction("Index");
        }
    }
}