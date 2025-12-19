using FitnessCenter.Models;
using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly IHizmetRepository _hizmetRepository;

        public HizmetController(IHizmetRepository hizmetRepository)
        {
            _hizmetRepository = hizmetRepository;
        }

        // --------------------- LISTE ---------------------
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }

        // --------------------- DETAILS ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- CREATE GET ---------------------
        public IActionResult Create()
        {
            return View();
        }

        // --------------------- CREATE POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet h)
        {
            if (!ModelState.IsValid)
                return View(h);

            await _hizmetRepository.AddAsync(h);

            TempData["Success"] = "Hizmet başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // --------------------- EDIT GET ---------------------
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- EDIT POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Hizmet h)
        {
            if (!ModelState.IsValid)
                return View(h);

            var mevcut = await _hizmetRepository.GetByIdAsync(h.Id);
            if (mevcut == null)
                return NotFound();

            // Mevcut tracked entity'yi güncelle
            mevcut.Ad = h.Ad;
            mevcut.Ucret = h.Ucret;
            mevcut.Sure = h.Sure;

            await _hizmetRepository.UpdateAsync(mevcut);

            TempData["Success"] = "Hizmet başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // --------------------- DELETE GET ---------------------
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- DELETE POST ---------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            await _hizmetRepository.RemoveAsync(hizmet);

            TempData["Delete"] = "Hizmet başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}