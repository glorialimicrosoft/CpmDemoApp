using System.Text;
using Microsoft.AspNetCore.Mvc;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Text.Json;
using CpmDemoApp.Models;
using CpmDemoApp.Util;
using CpmDemoApp.NugetOperations;
using Azure.Communication.Messages;
using Microsoft.Extensions.Options;

namespace viewer.Controllers
{
    [Route("webhook")]
    public class WebhookController : Controller
    {
        private bool EventTypeSubcriptionValidation
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "SubscriptionValidation";

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "Notification";

        private static string _channelRegistrationId;

        private JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public WebhookController(IOptions<ClientOptions> options)
        {
            _channelRegistrationId = options.Value.ChannelRegistrationId;
                
        }

        [HttpOptions]
        public async Task<IActionResult> Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                var webhookRequestCallback = HttpContext.Request.Headers["WebHook-Request-Callback"];
                var webhookRequestRate = HttpContext.Request.Headers["WebHook-Request-Rate"];
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var jsonContent = await reader.ReadToEndAsync();

                // Check the event type.
                // Return the validation code if it's 
                // a subscription validation request. 
                if (EventTypeSubcriptionValidation)
                {
                    return await HandleValidation(jsonContent);
                }
                else if (EventTypeNotification)
                {
                    return await HandleGridEvents(jsonContent);
                }

                return BadRequest();
            }
        }

        private async Task<JsonResult> HandleValidation(string jsonContent)
        {
            var eventGridEvent = JsonSerializer.Deserialize<EventGridEvent[]>(jsonContent, _options).First();
            var eventData = JsonSerializer.Deserialize<SubscriptionValidationEventData>(eventGridEvent.Data.ToString(), _options);
            var responseData = new SubscriptionValidationResponse
            {
                ValidationResponse = eventData.ValidationCode
            };
            return new JsonResult(responseData);
        }

        private async Task<IActionResult> HandleGridEvents(string jsonContent)
        {
            var eventGridEvents = JsonSerializer.Deserialize<EventGridEvent[]>(jsonContent, _options);
            foreach (var eventGridEvent in eventGridEvents)
            {
                if (eventGridEvent.EventType.ToLower() == "microsoft.communication.advancedmessagereceived") 
                {
                    var messageReceivedEventData = JsonSerializer.Deserialize<CrossPlatformMessageReceivedEventData>(eventGridEvent.Data.ToString(), _options);

                    if(messageReceivedEventData.From.Contains("conversationId", StringComparison.OrdinalIgnoreCase))
                    {
                        // we don't want to handle conversation messages received events, they are formatted as "{"from":"xxx","conversationId":"xxx"}"
                        break;
                    }

                    if (messageReceivedEventData.To.Equals(_channelRegistrationId, StringComparison.OrdinalIgnoreCase))
                    {
                        Data.NewIncomingMessagesListStatic.Add(new NewInboxItem
                        {
                            MessageContent = messageReceivedEventData?.Content,
                            CustomerPhoneNumber = messageReceivedEventData.From,
                            ArrivalTime = messageReceivedEventData.ReceivedTimeStamp,
                            Analysis = new AdvancedMessageAnalysisCompletedEventData
                            {
                                IntentAnalysis = "Placeholder",
                                ExtractedKeyPhrases = new List<string> { "palceholder" },
                            }
                        });
                    }
                }
                else if (eventGridEvent.EventType.ToLower() == "microsoft.communication.advancedmessageanalysiscompleted")
                {
                    var analysisCompletedEventData = JsonSerializer.Deserialize<AdvancedMessageAnalysisCompletedEventData>(eventGridEvent.Data.ToString(), _options);

                    if (analysisCompletedEventData.To.Equals(_channelRegistrationId, StringComparison.OrdinalIgnoreCase))
                    {
                        for (int i = Data.NewIncomingMessagesListStatic.Count - 1; i >= 0; i--)
                        {
                            var item = Data.NewIncomingMessagesListStatic[i];

                            // Check if the incoming event matches the current item
                            if (item.MessageContent == analysisCompletedEventData.OriginalMessage &&
                                item.CustomerPhoneNumber == analysisCompletedEventData.From)
                            {
                                // Fill the Analysis field with the incoming event
                                item.Analysis = analysisCompletedEventData;
                                break; // Exit the loop once the match is found
                            }
                        }
                    }
                }
            }

            return Ok();
        }
    }
}