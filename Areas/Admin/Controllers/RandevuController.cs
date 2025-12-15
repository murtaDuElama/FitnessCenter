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

        // 1) ÖZET LİSTE (Admin Panel Kartından gelinir)
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular); // Views/Randevu/Index.cshtml (özet)
        }

        // 2) TAM YÖNETİM LİSTESİ (Navbar "Randevular")
        public async Task<IActionResult> Manage()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular); // Views/Randevu/Manage.cshtml (tam yönetim)
        }

        // 3) DETAY (opsiyonel: istersen burada sadece detay + sil)
        public async Task<IActionResult> Edit(int id)
        {
            var randevu = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (randevu == null) return NotFound();
            return View(randevu);
        }

        // 4) SİL (tam yönetimden veya detaydan)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                TempData["Error"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Manage));
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            TempData["Delete"] = "Randevu başarıyla silindi!";
            return RedirectToAction(nameof(Manage));
        }
    }
}
