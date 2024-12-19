namespace CpmDemoApp.Models
{
    public class NewInboxItem
    {
        public string CustomerPhoneNumber { get; set; }

        public string MessageContent { get; set; }

        public AdvancedMessageAnalysisCompletedEventData Analysis { get; set; }

        public DateTimeOffset ArrivalTime { get; set; }
    }
}
