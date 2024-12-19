using CpmDemoApp.Util;

namespace CpmDemoApp.Models
{
    public class CreateConversationForm
    {
        public string CustomerPhoneNumber { get; set; }

        /// <summary>
        /// First inbound message from c2
        /// </summary>
        public string InitialIncomingMessage { get; set; }

        public List<string> SelectedAgentIds { get; set; }

        public List<Agent> SelectedAgents
        {
            get
            {
                return Data.AgentLists.Where(agent => SelectedAgentIds.Contains(agent.Id)).ToList();
            }
        }

        /// <summary>
        /// Messages between C2 and agents
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();

    }
}
