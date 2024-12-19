using CpmDemoApp.Models;

namespace CpmDemoApp.Util
{
    public static class Data
    {
        public static Dictionary<string, Customer> CustomerDB = new Dictionary<string, Customer>();
        public static Dictionary<string, Conversation> ConversationDB = new Dictionary<string, Conversation>();
        public static IList<NewInboxItem> NewIncomingMessagesListStatic { get; set; } = new List<NewInboxItem>();

        public static Dictionary<string, string> ExistingPhoneNumbers = new Dictionary<string, string>();

        public static List<Agent> AgentLists = new List<Agent>
        {
            new Agent { Id = "<Agent Mri here >", Name = "Sarah" },
            new Agent { Id = "<Agent Mri here >", Name = "Sam" },
            new Agent { Id = "<Agent Mri here >", Name = "Tom" },
            new Agent { Id = "<Agent Mri here >", Name = "Alison" },
            new Agent { Id = "<Agent Mri here >", Name = "Josh" },
        };
    }
}
