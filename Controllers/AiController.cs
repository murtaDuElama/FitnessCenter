using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IGeminiService _geminiService;

        public AIController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        // ==================== AI ASISTANI ANA SAYFA ====================
        public IActionResult Index()
        {
            return View();
        }

        // ==================== ANTRENMAN ANALİZİ ====================
        [HttpPost]
        public async Task<IActionResult> AnalyzeWorkout(string workoutDescription)
        {
            if (string.IsNullOrWhiteSpace(workoutDescription))
            {
                TempData["Error"] = "Lütfen antrenman açıklaması girin.";
                return RedirectToAction("Index");
            }

            var result = await _geminiService.AnalyzeWorkoutAsync(workoutDescription);

            ViewBag.WorkoutDescription = workoutDescription;
            ViewBag.Analysis = result;

            return View("Index");
        }

        // ==================== BESLENME TAVSİYESİ ====================
        [HttpPost]
        public async Task<IActionResult> GetNutritionAdvice(string nutritionQuery)
        {
            if (string.IsNullOrWhiteSpace(nutritionQuery))
            {
                TempData["Error"] = "Lütfen beslenme sorunuzu girin.";
                return RedirectToAction("Index");
            }

            var result = await _geminiService.GetNutritionAdviceAsync(nutritionQuery);

            ViewBag.NutritionQuery = nutritionQuery;
            ViewBag.NutritionAdvice = result;

            return View("Index");
        }

        // ==================== GENEL SORU-CEVAP ====================
        [HttpPost]
        public async Task<IActionResult> AskQuestion(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                TempData["Error"] = "Lütfen bir soru girin.";
                return RedirectToAction("Index");
            }

            var result = await _geminiService.GenerateTextAsync(question);

            ViewBag.Question = question;
            ViewBag.Answer = result;

            return View("Index");
        }
    }
}