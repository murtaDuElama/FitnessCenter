namespace FitnessCenter.Configuration
{
    public class AISettings
    {
        public string GeminiApiKey { get; set; } = string.Empty;
        public string GeminiModel { get; set; } = "gemini-1.5-flash";
        public string GeminiEndpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
    }
}