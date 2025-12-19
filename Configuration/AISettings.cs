namespace FitnessCenter.Configuration
{
    public class AISettings
    {
        public string GroqApiKey { get; set; } = "";
        public string GroqModel { get; set; } = "llama-3.1-8b-instant";
        public string GroqBaseUrl { get; set; } = "https://api.groq.com/openai/v1";
    }
}
