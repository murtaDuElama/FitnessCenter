using FitnessCenter.Models;
using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class HizmetController : Controller
    {
        private readonly IHizmetRepository _hizmetRepository;

        public HizmetController(IHizmetRepository hizmetRepository)
        {
            _hizmetRepository = hizmetRepository;
        }

        // Sadece listeleme yapar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }
    }
}
