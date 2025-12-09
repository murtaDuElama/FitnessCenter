using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // --------------------- LISTE (INDEX) ---------------------
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(x => x.Hizmet)
                .Include(x => x.Antrenor)
                .OrderByDescending(x => x.Tarih) // En yakın/yeni randevu en üstte
                .ThenBy(x => x.Saat)
                .ToListAsync();

            return View(randevular);
        }

        // --------------------- ONAYLA (POST) ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.Onaylandi = true;
                _context.Update(randevu); // Durumu güncelle
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu başarıyla onaylandı.";
            }
            return RedirectToAction("Index");
        }

        // --------------------- SIL (POST) ---------------------
        // Admin panelinde genelde hızlı silme için direkt POST kullanılır, 
        // ancak isterseniz onay sayfası (GET) da yapabiliriz. Şimdilik pratik olanı:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["DeleteMessage"] = "Randevu silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}