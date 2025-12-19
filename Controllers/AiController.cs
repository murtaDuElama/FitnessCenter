using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IAIService _ai;

        public AIController(IAIService ai)
        {
            _ai = ai ?? throw new ArgumentNullException(nameof(ai));
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnalyzeWorkout(string workoutDescription)
        {
            if (string.IsNullOrWhiteSpace(workoutDescription))
            {
                TempData["Error"] = "Lütfen antrenman açıklaması girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _ai.AnalyzeWorkoutAsync(workoutDescription);
                ViewBag.WorkoutDescription = workoutDescription;
                ViewBag.Analysis = result;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"AI isteği başarısız: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNutritionAdvice(string nutritionQuery)
        {
            if (string.IsNullOrWhiteSpace(nutritionQuery))
            {
                TempData["Error"] = "Lütfen beslenme sorunuzu girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                TempData["Error"] = "Lütfen bir soru girin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
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
