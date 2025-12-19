using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitnessCenter.Models;
using FitnessCenter.Repositories;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly IAntrenorRepository _antrenorRepository;
        private readonly IHizmetRepository _hizmetRepository;

        public AntrenorController(
            IAntrenorRepository antrenorRepository,
            IHizmetRepository hizmetRepository)
        {
            _antrenorRepository = antrenorRepository;
            _hizmetRepository = hizmetRepository;
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
        public async Task<IActionResult> Create()
        {
            // Hizmet adlarını dropdown için al
            var hizmetler = await _hizmetRepository.GetAllAsync();
            ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();

            return View();
        }

        // --------------------- CREATE POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Antrenor a)
        {
            if (!ModelState.IsValid)
            {
                // ViewBag'i doldur ve View'a geri dön
                var hizmetler = await _hizmetRepository.GetAllAsync();
                ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();
                return View(a);
            }

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

            // Hizmet adlarını dropdown için al
            var hizmetler = await _hizmetRepository.GetAllAsync();
            ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();

            return View(ant);
        }

        // --------------------- EDIT POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Antrenor a)
        {
            if (!ModelState.IsValid)
            {
                // ViewBag'i doldur ve View'a geri dön
                var hizmetler = await _hizmetRepository.GetAllAsync();
                ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();
                return View(a);
            }

            var mevcut = await _antrenorRepository.GetByIdAsync(a.Id);
            if (mevcut == null)
                return NotFound();

            // Mevcut tracked entity'yi güncelle
            mevcut.AdSoyad = a.AdSoyad;
            mevcut.Uzmanlik = a.Uzmanlik;
            mevcut.FotografUrl = a.FotografUrl;

            await _antrenorRepository.UpdateAsync(mevcut);

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