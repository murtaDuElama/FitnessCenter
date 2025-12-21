// =============================================================================
// DOSYA: GroqService.cs
// AÇIKLAMA: Groq AI servisi implementasyonu - yapay zeka metin üretimi
// NAMESPACE: FitnessCenter.Services
// API: Groq Cloud (https://console.groq.com)
// MODEL: Ayarlardan alınır (varsayılan: llama3-8b-8192)
// =============================================================================

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
    /// <summary>
    /// Groq AI servisi implementasyonu.
    /// IAIService interface'ini Groq Cloud API ile uygular.
    /// Antrenman analizi, beslenme önerileri ve genel metin üretimi sağlar.
    /// </summary>
    /// <remarks>
    /// Groq, hızlı LLM inference sağlayan bir platformdur.
    /// Ücretsiz tier ile sınırlı kullanım mümkündür.
    /// Rate limiting (429) için otomatik retry mekanizması içerir.
    /// </remarks>
    public class GroqService : IAIService
    {
        /// <summary>HttpClient - API istekleri için</summary>
        private readonly HttpClient _http;

        /// <summary>AI ayarları - API key, model, base URL</summary>
        private readonly AISettings _settings;

        /// <summary>
        /// GroqService constructor.
        /// HttpClient ve ayarları DI ile alır, header'ları yapılandırır.
        /// </summary>
        /// <param name="http">HttpClient (HttpClientFactory'den)</param>
        /// <param name="options">AI ayarları (appsettings.json'dan)</param>
        /// <exception cref="ArgumentNullException">Parametreler null ise</exception>
        /// <exception cref="InvalidOperationException">API key yapılandırılmamışsa</exception>
        public GroqService(HttpClient http, IOptions<AISettings> options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

            // API key kontrolü
            if (string.IsNullOrWhiteSpace(_settings.GroqApiKey))
                throw new InvalidOperationException("Groq API key yapılandırılmamış. AI:GroqApiKey değerini ayarlayın.");

            // HttpClient yapılandırması
            _http.BaseAddress = new Uri(_settings.GroqBaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.GroqApiKey);
        }

        // ===================== PUBLIC METODLAR =====================

        /// <summary>
        /// Antrenman programını analiz eder.
        /// Fitness koçu perspektifinden değerlendirme yapar.
        /// </summary>
        /// <param name="workoutDescription">Analiz edilecek antrenman açıklaması</param>
        /// <returns>Analiz sonucu (pozitifler, riskler, iyileştirmeler, örnek seans)</returns>
        public Task<string> AnalyzeWorkoutAsync(string workoutDescription)
        {
            var system = "You are a fitness coach. Give actionable, safe, concise guidance.";
            var user =
                "Analyze the following workout description. " +
                "Return: 1) positives 2) risks 3) improvements 4) sample next session.\n\n" +
                workoutDescription;

            return ChatAsync(system, user);
        }

        /// <summary>
        /// Beslenme sorusuna yanıt verir.
        /// Kanıta dayalı, tıbbi olmayan öneriler sunar.
        /// </summary>
        /// <param name="nutritionQuery">Beslenme sorusu</param>
        /// <returns>Beslenme önerileri</returns>
        public Task<string> GetNutritionAdviceAsync(string nutritionQuery)
        {
            var system = "You are a nutrition assistant. Give practical, evidence-based, non-medical guidance.";
            var user =
                "Answer the nutrition question concisely. " +
                "If missing info, ask up to 3 short clarification questions.\n\n" +
                nutritionQuery;

            return ChatAsync(system, user);
        }

        /// <summary>
        /// Genel metin üretir.
        /// Herhangi bir soruya kısa ve doğru yanıt verir.
        /// </summary>
        /// <param name="prompt">Kullanıcı promptu</param>
        /// <returns>AI yanıtı</returns>
        public Task<string> GenerateTextAsync(string prompt)
        {
            var system = "You are a helpful assistant. Be concise and correct.";
            return ChatAsync(system, prompt);
        }

        // ===================== PRİVATE METODLAR =====================

        /// <summary>
        /// Groq Chat Completion API'sine istek gönderir.
        /// Rate limiting için 3 kez retry yapar.
        /// </summary>
        /// <param name="systemPrompt">Sistem promptu (AI rolü)</param>
        /// <param name="userPrompt">Kullanıcı promptu</param>
        /// <returns>AI yanıtı</returns>
        /// <exception cref="Exception">API hatası veya retry sonrası başarısızlık</exception>
        private async Task<string> ChatAsync(string systemPrompt, string userPrompt)
        {
            // İstek payload'ı
            var req = new
            {
                model = _settings.GroqModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.2  // Düşük temperature = daha tutarlı yanıtlar
            };

            // Rate limit retry mekanizması (429 hatası için)
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                using var resp = await _http.PostAsJsonAsync("chat/completions", req);

                // 429 Too Many Requests - bekle ve tekrar dene
                if (resp.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = resp.Headers.RetryAfter?.Delta;
                    var delay = retryAfter ?? TimeSpan.FromSeconds(2 * attempt);
                    await Task.Delay(delay);
                    continue;
                }

                var json = await resp.Content.ReadAsStringAsync();

                // Hata durumunda exception fırlat
                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Groq API Hatası: {(int)resp.StatusCode} - {json}");

                // Yanıtı parse et ve content'i döndür
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
