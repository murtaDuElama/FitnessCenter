// =============================================================================
// DOSYA: AiController.cs
// AÇIKLAMA: AI destekli antrenman ve beslenme önerileri controller'ı
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /AI/Index, /AI/AnalyzeWorkout, /AI/GetNutritionAdvice, /AI/AskQuestion
// GEREKSİNİM: Giriş yapmış kullanıcılar (Authorize)
// =============================================================================

using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// AI entegrasyonu controller sınıfı.
    /// Groq AI API'si ile antrenman analizi, beslenme önerisi ve genel soru-cevap sağlar.
    /// Sadece giriş yapmış kullanıcılar erişebilir.
    /// </summary>
    [Authorize]  // Tüm action'lar için yetkilendirme gerekli
    public class AIController : Controller
    {
        /// <summary>
        /// AI servisi - Groq API ile iletişim kurar
        /// </summary>
        private readonly IAIService _ai;

        /// <summary>
        /// AIController constructor.
        /// Dependency Injection ile IAIService alır.
        /// </summary>
        /// <param name="ai">AI servis instance'ı</param>
        /// <exception cref="ArgumentNullException">ai parametresi null ise</exception>
        public AIController(IAIService ai)
        {
            _ai = ai ?? throw new ArgumentNullException(nameof(ai));
        }

        // ===================== ANA SAYFA =====================

        /// <summary>
        /// AI asistan ana sayfası.
        /// GET: /AI veya /AI/Index
        /// Tüm AI özelliklerinin form arayüzünü içerir.
        /// </summary>
        /// <returns>AI Index view'ı</returns>
        [HttpGet]
        public IActionResult Index() => View();

        // ===================== ANTRENMAN ANALİZİ =====================

        /// <summary>
        /// Antrenman programı analizi.
        /// POST: /AI/AnalyzeWorkout
        /// Kullanıcının antrenman açıklamasını AI'ya gönderir ve analiz alır.
        /// </summary>
        /// <param name="workoutDescription">Analiz edilecek antrenman açıklaması</param>
        /// <returns>Analiz sonucu ile Index view'ı</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnalyzeWorkout(string workoutDescription)
        {
            // Boş girdi kontrolü
            if (string.IsNullOrWhiteSpace(workoutDescription))
            {
                TempData["Error"] = "Lütfen antrenman açıklaması girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // AI servisinden analiz al
                var result = await _ai.AnalyzeWorkoutAsync(workoutDescription);

                // Sonuçları ViewBag ile gönder
                ViewBag.WorkoutDescription = workoutDescription;
                ViewBag.Analysis = result;
                return View("Index");
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıyı bilgilendir
                TempData["Error"] = $"AI isteği başarısız: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================== BESLENME ÖNERİSİ =====================

        /// <summary>
        /// Beslenme önerisi alma.
        /// POST: /AI/GetNutritionAdvice
        /// Kullanıcının beslenme sorusuna AI'dan cevap alır.
        /// </summary>
        /// <param name="nutritionQuery">Beslenme ile ilgili soru</param>
        /// <returns>Beslenme önerisi ile Index view'ı</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNutritionAdvice(string nutritionQuery)
        {
            // Boş girdi kontrolü
            if (string.IsNullOrWhiteSpace(nutritionQuery))
            {
                TempData["Error"] = "Lütfen beslenme sorunuzu girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // AI servisinden beslenme önerisi al
                var result = await _ai.GetNutritionAdviceAsync(nutritionQuery);

                ViewBag.NutritionQuery = nutritionQuery;
                ViewBag.NutritionAdvice = result;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"AI isteği başarısız: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================== GENEL SORU-CEVAP =====================

        /// <summary>
        /// Genel soru sorma.
        /// POST: /AI/AskQuestion
        /// Fitness ile ilgili herhangi bir soruya AI'dan cevap alır.
        /// </summary>
        /// <param name="question">Kullanıcının sorusu</param>
        /// <returns>Cevap ile Index view'ı</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion(string question)
        {
            // Boş girdi kontrolü
            if (string.IsNullOrWhiteSpace(question))
            {
                TempData["Error"] = "Lütfen bir soru girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // AI servisinden genel cevap al
                var result = await _ai.GenerateTextAsync(question);

                ViewBag.Question = question;
                ViewBag.Answer = result;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"AI isteği başarısız: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
