using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly IAntrenorRepository _antrenorRepository;

        public AntrenorController(IAntrenorRepository antrenorRepository)
        {
            _antrenorRepository = antrenorRepository;
        }

        // --------------------- LISTE ---------------------
        public async Task<IActionResult> Index()
        {
            var liste = await _antrenorRepository.GetAllAsync();
            return View(liste);
        }

        // --------------------- DETAILS ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- CREATE GET ---------------------
        public IActionResult Create()
        {
            return View();
        }

        // --------------------- CREATE POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Antrenor a)
        {
            if (!ModelState.IsValid)
                return View(a);

            await _antrenorRepository.AddAsync(a);

            TempData["Success"] = "Antrenör başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // --------------------- EDIT GET ---------------------
        public async Task<IActionResult> Edit(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- EDIT POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Antrenor a)
        {
            if (!ModelState.IsValid)
                return View(a);

            var mevcut = await _antrenorRepository.GetByIdAsync(a.Id);
            if (mevcut == null)
                return NotFound();

            await _antrenorRepository.UpdateAsync(a);

            TempData["Success"] = "Antrenör başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // --------------------- DELETE GET ---------------------
        public async Task<IActionResult> Delete(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- DELETE POST ---------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            await _antrenorRepository.RemoveAsync(ant);

            TempData["Delete"] = "Antrenör başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}
