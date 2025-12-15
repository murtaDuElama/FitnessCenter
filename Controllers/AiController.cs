using FitnessCenter.Models;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class AiController : Controller
    {
        private readonly AiService _aiService;

        public AiController(AiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Analyze(AIRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var response = _aiService.BuildPlan(model);
            return View("Result", response);
        }
    }
}
