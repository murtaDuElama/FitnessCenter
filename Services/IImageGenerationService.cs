// =============================================================================
// DOSYA: IImageGenerationService.cs
// AÇIKLAMA: Görsel üretme servisi interface - AI ile egzersiz görseli oluşturma
// NAMESPACE: FitnessCenter.Services
// KULLANIM: ImageGenerationController tarafından egzersiz görselleri üretmek için
// =============================================================================

using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    /// <summary>
    /// Görsel üretme servisi interface.
    /// AI destekli egzersiz görseli oluşturma için sözleşme tanımlar.
    /// </summary>
    /// <remarks>
    /// Implementasyon: PollinationsService
    /// - Pollinations.ai API kullanır
    /// - Ücretsiz, API key gerektirmez
    /// - URL tabanlı görsel üretimi
    /// </remarks>
    public interface IImageGenerationService
    {
        /// <summary>
        /// Egzersiz bilgilerine göre AI destekli görsel üretir.
        /// </summary>
        /// <param name="exerciseName">
        /// Egzersiz adı.
        /// Örnek değerler: "Squat", "Bench Press", "Deadlift", "Push-up"
        /// </param>
        /// <param name="days">
        /// Haftalık antrenman gün sayısı (1-7).
        /// Prompt oluşturmada kullanılır.
        /// </param>
        /// <param name="reps">
        /// Set başına tekrar sayısı.
        /// Prompt oluşturmada kullanılır.
        /// </param>
        /// <returns>
        /// Üretilen görselin URL'i.
        /// Bu URL doğrudan img src olarak kullanılabilir.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// exerciseName boş veya null ise fırlatılır.
        /// </exception>
        Task<string> GenerateExerciseImageAsync(string exerciseName, int days, int reps);
    }
}
