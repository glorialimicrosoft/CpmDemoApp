using CpmDemoApp.Util;

namespace CpmDemoApp.Models
{
    public class Conversation
    {
        public string Id { get; set; }

        public List<string> SelectedAgentIds { get; set; }

        public List<Agent> SelectedAgents
        {
            get
            {
                return Data.AgentLists.Where(agent => SelectedAgentIds.Contains(agent.Id)).ToList();
            }
        }
        public Customer ConversationCustomer { get; set; }

        public string InitialIncomingMessage { get; set; }

        /// <summary>
        /// Messages between C2 and agents
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();

    }
}
