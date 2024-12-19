namespace CpmDemoApp.Models
{
    public class AdvancedMessageAnalysisCompletedEventData
    {
        public string OriginalMessage { get; set; }

        public LanguageDetection LanguageDetection { get; set; }

        public string IntentAnalysis { get; set; }

        public List<string> ExtractedKeyPhrases { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTimeOffset ReceivedTimeStamp { get; set; }
    }
}
