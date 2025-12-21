// =============================================================================
// DOSYA: IAIService.cs
// AÇIKLAMA: AI servisi interface - yapay zeka işlemleri için sözleşme
// NAMESPACE: FitnessCenter.Services
// KULLANIM: AIController tarafından antrenman analizi ve beslenme önerileri için
// =============================================================================

using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    /// <summary>
    /// AI servisi interface.
    /// Yapay zeka destekli antrenman analizi, beslenme önerileri
    /// ve genel metin üretimi için sözleşme tanımlar.
    /// </summary>
    /// <remarks>
    /// Implementasyon: GroqService (Groq AI API kullanır)
    /// Alternatif implementasyonlar: OpenAI, Claude vb.
    /// </remarks>
    public interface IAIService
    {
        /// <summary>
        /// Antrenman programını analiz eder ve öneriler sunar.
        /// </summary>
        /// <param name="workoutDescription">Analiz edilecek antrenman açıklaması</param>
        /// <returns>
        /// AI yanıtı içerir:
        /// - Pozitif yönler
        /// - Riskler
        /// - İyileştirme önerileri
        /// - Örnek sonraki seans
        /// </returns>
        Task<string> AnalyzeWorkoutAsync(string workoutDescription);

        /// <summary>
        /// Beslenme sorusuna yanıt verir.
        /// </summary>
        /// <param name="nutritionQuery">Beslenme ile ilgili soru</param>
        /// <returns>Kanıta dayalı, pratik beslenme önerileri</returns>
        Task<string> GetNutritionAdviceAsync(string nutritionQuery);

        /// <summary>
        /// Genel metin üretir (soru-cevap).
        /// </summary>
        /// <param name="prompt">Kullanıcı promptu</param>
        /// <returns>AI tarafından üretilen yanıt</returns>
        Task<string> GenerateTextAsync(string prompt);
    }
}
