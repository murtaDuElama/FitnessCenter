using FitnessCenter.Configuration;
using FitnessCenter.Models.Gemini;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly AISettings _settings;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GeminiService(HttpClient httpClient, IOptions<AISettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
                return "API anahtarı yapılandırılmamış. User Secrets veya Environment Variables ile GeminiApiKey ayarlayın.";

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url =
                    $"{_settings.GeminiEndpoint}/models/{_settings.GeminiModel}:generateContent?key={_settings.GeminiApiKey}";

                using var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return $"API Hatası: {response.StatusCode} - {error}";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson, _jsonOptions);

                return result?.Candidates?
                           .FirstOrDefault()?
                           .Content?
                           .Parts?
                           .FirstOrDefault()?
                           .Text
                       ?? "Yanıt alınamadı.";
            }
            catch (Exception ex)
            {
                return $"Hata: {ex.Message}";
            }
        }

        public Task<string> AnalyzeWorkoutAsync(string workoutDescription)
        {
            var prompt = $@"
Sen bir fitness uzmanısın. Aşağıdaki antrenman programını analiz et ve şu bilgileri ver:

1. Antrenmanın genel değerlendirmesi
2. Güçlü yönleri
3. İyileştirme önerileri
4. Tavsiye edilen süre ve set/tekrar sayıları
5. Dikkat edilmesi gereken noktalar

Antrenman Açıklaması:
{workoutDescription}

Lütfen yanıtını Türkçe olarak, maddeler halinde ve açık bir şekilde ver.
";
            return GenerateTextAsync(prompt);
        }

        public Task<string> GetNutritionAdviceAsync(string userQuery)
        {
            var prompt = $@"
Sen bir beslenme uzmanısın. Kullanıcının beslenme ile ilgili sorusuna detaylı ve bilimsel bir yanıt ver.

Kullanıcı Sorusu:
{userQuery}

Yanıtında şunlara dikkat et:
1. Bilimsel ve güvenilir bilgi ver
2. Kişiye özel sağlık durumları için doktora danışmasını öner
3. Pratik ve uygulanabilir tavsiyeler sun
4. Türkçe ve anlaşılır bir dil kullan

Yanıtını yapılandır ve maddeler halinde sun.
";
            return GenerateTextAsync(prompt);
        }
    }
}
