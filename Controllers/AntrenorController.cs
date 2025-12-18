using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class AntrenorController : Controller
    {
        private readonly IAntrenorRepository _antrenorRepository;

        public AntrenorController(IAntrenorRepository antrenorRepository)
        {
            _antrenorRepository = antrenorRepository;
        }

        // Public antrenör listesi
        public async Task<IActionResult> Index()
        {
            var antrenorler = await _antrenorRepository.GetAllAsync();
            return View(antrenorler);
        }
    }
}
