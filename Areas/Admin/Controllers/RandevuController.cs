using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;

        public RandevuController(AppDbContext context)
        {
            _context = context;
        }

        // --------------------- LİSTELEME ---------------------
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(x => x.Hizmet)
                .Include(x => x.Antrenor)
                .OrderByDescending(x => x.Tarih)
                .ThenBy(x => x.Saat)
                .ToListAsync();

            return View(randevular);
        }

        // --------------------- DÜZENLEME (GET) ---------------------
        public async Task<IActionResult> Edit(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            // Dropdownlar için verileri doldur
            ViewBag.Hizmetler = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            ViewBag.Antrenorler = new SelectList(_context.Antrenorler, "Id", "AdSoyad", randevu.AntrenorId);

            return View(randevu);
        }

        // --------------------- DÜZENLEME (POST) ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Randevu randevu)
        {
            if (id != randevu.Id)
            {
                return NotFound();
            }

            // Model validasyonunda User, Hizmet ve Antrenör navigation propertyleri null gelebilir, bunları ignore ediyoruz
            ModelState.Remove("Hizmet");
            ModelState.Remove("Antrenor");
            ModelState.Remove("UserId"); // Admin düzenlerken UserId değişmez

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Randevu başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Randevular.Any(e => e.Id == randevu.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa dropdownları tekrar doldur
            ViewBag.Hizmetler = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            ViewBag.Antrenorler = new SelectList(_context.Antrenorler, "Id", "AdSoyad", randevu.AntrenorId);
            return View(randevu);
        }

        // --------------------- SİLME (POST) ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["Delete"] = "Randevu silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // --------------------- ONAYLAMA (POST) ---------------------
        [HttpPost]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.Onaylandi = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu onaylandı.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}