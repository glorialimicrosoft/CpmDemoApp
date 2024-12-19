namespace CpmDemoApp.Models
{
    public class Message
    {
        // Sender name or number
        public string From{ get; set; }

        public string SenderId{ get; set; }

        public string MessageContent { get; set; }

        public long SequenceId { get; set; }

        public bool IsSender { get; set; }

        public AdvancedMessageAnalysisCompletedEventData Analysis { get; set; }

        public DateTimeOffset ArrivalTime { get; set; }
    }
}
