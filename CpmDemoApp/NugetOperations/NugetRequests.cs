using Azure;
using Azure.Communication.Messages;
using Azure.Core;
using Azure.Core.Pipeline;
using CpmDemoApp.Mappers;
using CpmDemoApp.Models;
using CpmDemoApp.Util;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace CpmDemoApp.NugetOperations
{
    public class NugetRequests
    {
        private ConversationManagementClient _conversationManagementClient;

        public NugetRequests()
        {
            var options = ConfigurationHelper.GetClientOptions();
            _conversationManagementClient = new ConversationManagementClient(options.ConnectionString);
        }

        public async Task<string> CreateConversation(Conversation conversation)
        {
            var response = await _conversationManagementClient.CreateAsync(ConversationMapper.MapConversationRequestPayload(conversation));
            return response?.Value?.Id;
        }

        public async Task<bool> TerminateConversation(string conversationId)
        {
            var response = await _conversationManagementClient.DeleteConversationAsync(conversationId);
            // check delete status
            return !response.IsError;
        }

        public async Task<List<Conversation>> GetConversationsForEmployee(string employeeId)
        {
            var result = new List<Conversation>();
            var response = _conversationManagementClient.GetConversationsAsync(employeeId);
            await foreach (var conversationItem in response)
            {
                result.Add(ConversationMapper.MapFromConversationItem(conversationItem));
            }

            return result; 
        }

        public async Task<bool> AddAgent(string conversationId, List<string> selectedIds) 
        {
            var response = await _conversationManagementClient.AddAsync(conversationId, new AddEmployeesRequestPayload(ConversationMapper.GetEmployees(selectedIds)));
            return !response.IsError;
        }

        public async Task<bool> RemoveAgent(string conversationId, List<string> selectedIds)
        {
            var response = await _conversationManagementClient.RemoveAsync(conversationId, new RemoveEmployeesRequestPayload(selectedIds));
            return !response.IsError;
        }

        public async Task<bool> Terminate(string conversationId)
        {
            var removeAgentsResponse = await _conversationManagementClient.RemoveAsync(conversationId, new RemoveEmployeesRequestPayload(Data.ConversationDB[conversationId].SelectedAgentIds));
            var response = await _conversationManagementClient.DeleteConversationAsync(conversationId);
            return !response.IsError;
        }
    }
}
