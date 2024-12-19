using Azure.Communication.Messages;
using CpmDemoApp.Models;
using CpmDemoApp.Util;
using Microsoft.Extensions.Options;
using ConversationModel = CpmDemoApp.Models.Conversation;

namespace CpmDemoApp.Mappers
{
    public static class ConversationMapper
    {
        public static CreateConversationRequestPayload MapConversationRequestPayload(Conversation conversation)
        {
            var payload = new CreateConversationRequestPayload(MapConversation(conversation));
            payload.ElevatedMessage = new ConversationMessage
            {
                Content = conversation.InitialIncomingMessage,
            };
            return payload;
        }

        public static ConversationItem MapConversation(ConversationModel conversationModel)
        {
            var options = ConfigurationHelper.GetClientOptions();

            var recipient = MapRecipient(conversationModel.ConversationCustomer);

            var deliveryChannelIds = new List<string> { options.ChannelRegistrationId };

            var conversationItem = new ConversationItem
            ("id", recipient, deliveryChannelIds, OutboundDeliveryStrategyKind.AllChannels);


            // Add Employees
            foreach (var emp in GetEmployees(conversationModel.SelectedAgentIds))
            {
                conversationItem.Employees.Add(emp);
            }
            
            return conversationItem;
        }

        public static List<ConversationEmployee> GetEmployees(List<string> selectedIds) 
        {
            var options = ConfigurationHelper.GetClientOptions();
            var employees = new List<ConversationEmployee>();
            foreach (var selectedId in selectedIds)
            {
                employees.Add(new ConversationEmployee(selectedId));
            }
            return employees;
        }

        public static ConversationRecipient MapRecipient(Customer customer) 
        {
            var contactList = new List<ConversationContact> {
                new ConversationContact( customer.PhoneNumber, MessagePlatformKind.WhatsApp)
            };

            return new ConversationRecipient(contactList)
            {
                DisplayName = customer.Name,

            };

        }

        public static Message MapFromConversationMessageItem(ConversationMessageItem messageItem)
        {
            return new Message
            {
                SenderId = messageItem.SenderCommunicationIdentifier,
                MessageContent = messageItem.Message.Content,
                SequenceId = messageItem.SequenceId ?? 0,
            };
        }

        public static Conversation MapFromConversationItem(ConversationItem conversationItem) 
        {
            var conversation = new Conversation
            {
               Id = conversationItem.Id,
               ConversationCustomer = new Customer
               {
                   PhoneNumber =  conversationItem.Recipient.Contacts.FirstOrDefault(c => c.MessagePlatform == MessagePlatformKind.WhatsApp)?.Id,
               },
            };

            foreach(var employee in conversationItem.Employees)
            {
                conversation.SelectedAgentIds.Add(employee.Id);
            }

            return conversation;
        }

        public static NotificationContent MapNotificationContent(Message message) 
        {
            var options = ConfigurationHelper.GetClientOptions();
            
            var content = new TextNotificationContent(Guid.Parse(options.ChannelRegistrationId), null, message.MessageContent);

            return content;
        }
    }
}