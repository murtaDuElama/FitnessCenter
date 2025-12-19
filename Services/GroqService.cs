using FitnessCenter.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    public class GroqService : IAIService
    {
        private readonly HttpClient _http;
        private readonly AISettings _settings;

        public GroqService(HttpClient http, IOptions<AISettings> options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(_settings.GroqApiKey))
                throw new InvalidOperationException("Groq API key yapılandırılmamış. AI:GroqApiKey değerini ayarlayın.");

            _http.BaseAddress = new Uri(_settings.GroqBaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.GroqApiKey);
        }

        public Task<string> AnalyzeWorkoutAsync(string workoutDescription)
        {
            var system = "You are a fitness coach. Give actionable, safe, concise guidance.";
            var user =
                "Analyze the following workout description. " +
                "Return: 1) positives 2) risks 3) improvements 4) sample next session.\n\n" +
                workoutDescription;

            return ChatAsync(system, user);
        }

        public Task<string> GetNutritionAdviceAsync(string nutritionQuery)
        {
            var system = "You are a nutrition assistant. Give practical, evidence-based, non-medical guidance.";
            var user =
                "Answer the nutrition question concisely. " +
                "If missing info, ask up to 3 short clarification questions.\n\n" +
                nutritionQuery;

            return ChatAsync(system, user);
        }

        public Task<string> GenerateTextAsync(string prompt)
        {
            var system = "You are a helpful assistant. Be concise and correct.";
            return ChatAsync(system, prompt);
        }

        private async Task<string> ChatAsync(string systemPrompt, string userPrompt)
        {
            var req = new
            {
                model = _settings.GroqModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.2
            };

            // 429 için basit retry (rate-limit)
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                using var resp = await _http.PostAsJsonAsync("chat/completions", req);

                if (resp.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // Retry-After varsa kullan, yoksa artan bekleme
                    var retryAfter = resp.Headers.RetryAfter?.Delta;
                    var delay = retryAfter ?? TimeSpan.FromSeconds(2 * attempt);
                    await Task.Delay(delay);
                    continue;
                }

                var json = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Groq API Hatası: {(int)resp.StatusCode} - {json}");

                using var doc = JsonDocument.Parse(json);
                var content = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return content ?? "";
            }

            throw new Exception("Groq API: 429 TooManyRequests (retry sonrası da devam ediyor).");
        }
    }
}
