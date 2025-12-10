using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenter.Controllers
{
    [Authorize] // Sadece giriş yapmış üyeler erişebilir
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------ RANDEVU FORMU (GET) ------------------
        public IActionResult Create()
        {
            ViewBag.Hizmetler = new SelectList(_context.Hizmetler.ToList(), "Id", "Ad");
            ViewBag.Antrenorler = new SelectList(_context.Antrenorler.ToList(), "Id", "AdSoyad");
            return View();
        }

        // ------------------ RANDEVU KAYDETME (POST) ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            // Giriş yapan kullanıcıyı bul
            var user = await _userManager.GetUserAsync(User);
            randevu.UserId = user.Id;
            randevu.AdSoyad = user.AdSoyad ?? "İsimsiz Üye"; // Eğer modelden gelmezse kullanıcının adını al

            // "Direkt alınsın" dediğin için Onay durumunu TRUE yapabiliriz veya 
            // Admin onayı beklenecekse FALSE kalabilir. Senin isteğine göre TRUE yapıyorum:
            randevu.Onaylandi = true;

            // 1. KONTROL: Aynı Antrenörde, Aynı Tarih ve Saatte randevu var mı?
            bool musaitDegil = await _context.Randevular.AnyAsync(x =>
                x.AntrenorId == randevu.AntrenorId &&
                x.Tarih == randevu.Tarih &&
                x.Saat == randevu.Saat);

            if (musaitDegil)
            {
                ModelState.AddModelError("", "Seçtiğiniz antrenörün bu tarih ve saatte başka bir randevusu mevcut. Lütfen başka bir saat seçiniz.");
            }

            // 2. KONTROL: Kullanıcının aynı saatte başka randevusu var mı? (İsteğe bağlı)
            bool kullaniciDolu = await _context.Randevular.AnyAsync(x =>
                x.UserId == user.Id &&
                x.Tarih == randevu.Tarih &&
                x.Saat == randevu.Saat);

            if (kullaniciDolu)
            {
                ModelState.AddModelError("", "Bu saatte zaten başka bir randevunuz var.");
            }

            if (ModelState.IsValid)
            {
                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevunuz başarıyla oluşturuldu.";
                return RedirectToAction("MyRandevus");
            }

            // Hata varsa formu tekrar doldur
            ViewBag.Hizmetler = new SelectList(_context.Hizmetler.ToList(), "Id", "Ad");
            ViewBag.Antrenorler = new SelectList(_context.Antrenorler.ToList(), "Id", "AdSoyad");
            return View(randevu);
        }

        // ------------------ RANDEVULARIM (LİSTE) ------------------
        public async Task<IActionResult> MyRandevus()
        {
            var userId = _userManager.GetUserId(User);

            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Where(r => r.UserId == userId) // Sadece kendi randevuları
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular);
        }

        // ------------------ RANDEVU İPTAL ET (KULLANICI) ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            // Güvenlik Kontrolü: Sadece randevunun sahibi iptal edebilir
            if (randevu != null && randevu.UserId == userId)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["Info"] = "Randevu iptal edildi.";
            }
            else
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
            }

            return RedirectToAction("MyRandevus");
        }
    }
}