using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------ 1) HİZMET SEÇ ------------------
        public IActionResult SelectService()
        {
            var hizmetler = _context.Hizmetler.ToList();
            return View(hizmetler);
        }

        // ------------------ 2) HİZMETE GÖRE ANTRENÖR SEÇ ------------------
        public IActionResult SelectTrainer(int hizmetId)
        {
            var hizmet = _context.Hizmetler.Find(hizmetId);

            if (hizmet == null)
                return NotFound();

            // Uzmanlık alanına göre eğitmenleri filtrele
            var antrenorler = _context.Antrenorler
                .Where(a => a.Uzmanlik == hizmet.Ad)
                .ToList();

            ViewBag.Hizmet = hizmet;
            return View(antrenorler);
        }

        // ------------------ 3) ANTRENÖR MÜSAİTLİK SAATLERİ ------------------
        public IActionResult SelectTime(int hizmetId, int antrenorId)
        {
            // Tanımlı saat aralıkları
            var allHours = new List<string>
            {
                "09:00","10:00","11:00","13:00","14:00","15:00"
            };

            // Bugün o eğitmenin dolu saatlerini çekiyoruz
            var doluSaatler = _context.Randevular
                .Where(r => r.AntrenorId == antrenorId && r.Tarih.Date == DateTime.Today)
                .Select(r => r.Saat)
                .ToList();

            // Müsait saatler
            var musait = allHours.Where(h => !doluSaatler.Contains(h)).ToList();

            ViewBag.HizmetId = hizmetId;
            ViewBag.AntrenorId = antrenorId;

            return View(musait);
        }
        
        // ------------------ 4) RANDEVU OLUŞTUR ------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int hizmetId, int antrenorId, string saat)
        {
            if (string.IsNullOrWhiteSpace(saat))
            {
                TempData["Error"] = "Lütfen bir saat seçiniz.";
                return RedirectToAction("SelectTime", new { hizmetId, antrenorId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // 1. Antrenör o saatte dolu mu?
            bool dolu = await _context.Randevular.AnyAsync(x =>
                x.AntrenorId == antrenorId &&
                x.Tarih.Date == DateTime.Today &&
                x.Saat == saat);

            if (dolu)
            {
                TempData["Error"] = "Bu saat dolu! Lütfen başka bir saat seçiniz.";
                return RedirectToAction("SelectTime", new { hizmetId, antrenorId });
            }

            // 2. Kullanıcının kendi çakışması var mı?
            bool kullaniciDolu = await _context.Randevular.AnyAsync(x =>
                x.UserId == user.Id &&
                x.Tarih.Date == DateTime.Today &&
                x.Saat == saat);

            if (kullaniciDolu)
            {
                TempData["Error"] = "Bu saatte zaten başka bir randevunuz var!";
                return RedirectToAction("SelectTime", new { hizmetId, antrenorId });
            }

            var randevu = new Randevu
            {
                UserId = user.Id,
                HizmetId = hizmetId,
                AntrenorId = antrenorId,
                Tarih = DateTime.Today,
                Saat = saat,
                Onaylandi = true,
                AdSoyad = user.AdSoyad ?? user.UserName
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevunuz başarıyla oluşturuldu.";
            return RedirectToAction("MyRandevus");
        }


        // ------------------ 5) RANDEVULARIM ------------------
        public async Task<IActionResult> MyRandevus()
        {
            var userId = _userManager.GetUserId(User);

            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular);
        }

        // ------------------ 6) RANDEVU İPTAL ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            var userId = _userManager.GetUserId(User);

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
