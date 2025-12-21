// =============================================================================
// DOSYA: PollinationsService.cs
// AÇIKLAMA: Pollinations.ai servisi - ücretsiz AI görsel üretimi
// NAMESPACE: FitnessCenter.Services
// API: Pollinations.ai (https://pollinations.ai) - API key gerektirmez
// =============================================================================

using System;
using System.Net;
using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    /// <summary>
    /// Pollinations.ai görsel üretme servisi.
    /// IImageGenerationService interface'ini ücretsiz Pollinations API ile uygular.
    /// </summary>
    /// <remarks>
    /// Pollinations.ai özellikleri:
    /// - Tamamen ücretsiz
    /// - API key gerektirmez
    /// - URL tabanlı - prompt URL'e encode edilir
    /// - Sonuç URL'i doğrudan img src olarak kullanılabilir
    /// </remarks>
    public class PollinationsService : IImageGenerationService
    {
        /// <summary>Pollinations.ai base URL</summary>
        private const string BaseUrl = "https://image.pollinations.ai/prompt/";

        /// <summary>
        /// Pollinations.ai API'sini kullanarak egzersiz görseli üretir.
        /// URL tabanlı - prompt encode edilir ve görsel URL'i döner.
        /// </summary>
        /// <param name="exerciseName">Egzersiz adı</param>
        /// <param name="days">Haftalık gün sayısı</param>
        /// <param name="reps">Tekrar sayısı</param>
        /// <returns>Üretilen görselin URL'i</returns>
        /// <exception cref="ArgumentException">exerciseName boşsa</exception>
        public Task<string> GenerateExerciseImageAsync(string exerciseName, int days, int reps)
        {
            if (string.IsNullOrWhiteSpace(exerciseName))
                throw new ArgumentException("Egzersiz adı boş olamaz.", nameof(exerciseName));

            // Fitness odaklı detaylı prompt oluştur
            var prompt = CreateFitnessPrompt(exerciseName, days, reps);

            // URL encode edilmiş prompt
            var encodedPrompt = WebUtility.UrlEncode(prompt);

            // Pollinations.ai görsel URL'i oluştur
            // Parametreler: width, height, nologo
            var imageUrl = $"{BaseUrl}{encodedPrompt}?width=768&height=512&nologo=true";

            return Task.FromResult(imageUrl);
        }

        // ===================== PRİVATE METODLAR =====================

        /// <summary>
        /// Egzersiz bilgilerinden detaylı ve görsel açıklayıcı prompt oluşturur.
        /// AI'ın kaliteli görsel üretmesi için optimize edilmiş prompt.
        /// </summary>
        /// <param name="exerciseName">Egzersiz adı</param>
        /// <param name="days">Haftalık gün sayısı</param>
        /// <param name="reps">Tekrar sayısı</param>
        /// <returns>AI görsel üretimi için optimize edilmiş prompt</returns>
        private string CreateFitnessPrompt(string exerciseName, int days, int reps)
        {
            // Egzersiz türüne göre ek detaylar
            var exerciseDetails = GetExerciseDetails(exerciseName);

            // Detaylı, görsel odaklı prompt
            var prompt = $"Professional fitness illustration showing a fit athletic person performing {exerciseName} exercise, " +
                         $"{exerciseDetails}, " +
                         $"training schedule {days} days per week with {reps} repetitions, " +
                         $"modern gym environment with professional lighting, " +
                         $"motivational fitness poster style, " +
                         $"high quality digital art, vibrant colors, dynamic pose, " +
                         $"showing proper form and technique";

            return prompt;
        }

        /// <summary>
        /// Her egzersiz türü için özel görsel detaylar döndürür.
        /// Bu detaylar görselin daha spesifik ve doğru olmasını sağlar.
        /// </summary>
        /// <param name="exerciseName">Egzersiz adı</param>
        /// <returns>Egzersize özel görsel açıklama</returns>
        private string GetExerciseDetails(string exerciseName)
        {
            // Egzersiz adına göre özel detaylar
            return exerciseName.ToLower() switch
            {
                "squat" or "skuat" => "barbell on shoulders, deep squat position, strong leg muscles visible",
                "bench press" => "lying on bench, pressing barbell upward, chest muscles engaged",
                "deadlift" => "lifting heavy barbell from ground, back straight, powerful stance",
                "push-up" or "şınav" => "plank position, arms extended, core tight, bodyweight exercise",
                "pull-up" or "barfiks" => "hanging from bar, pulling body upward, back muscles defined",
                "plank" => "static hold position, core engaged, perfect body alignment",
                "bicep curl" => "holding dumbbells, curling weights, bicep muscles flexed",
                "lunges" => "stepping forward, deep lunge position, leg muscles working",
                "shoulder press" or "omuz press" => "pressing dumbbells overhead, shoulder muscles defined",
                "lat pulldown" => "seated at cable machine, pulling bar down, wide back muscles",
                "leg press" => "seated on machine, pressing weight with legs, quadriceps engaged",
                "tricep dips" => "on parallel bars or bench, lowering body, triceps working",
                "crunches" => "lying on mat, curling upper body, abdominal muscles contracting",
                "burpees" => "explosive full body movement, jumping and dropping to ground",
                "kettlebell swing" => "swinging kettlebell, hip hinge movement, full body exercise",
                _ => "proper exercise form, focused athletic movement, muscle engagement visible"
            };
        }
    }
}
