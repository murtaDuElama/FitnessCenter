using FitnessCenter.Data;
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

        // RANDEVU LİSTESİ
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular);
        }

        // ONAYLA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                TempData["Delete"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            randevu.Onaylandi = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        // SİL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                TempData["Delete"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            TempData["Delete"] = "Randevu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
