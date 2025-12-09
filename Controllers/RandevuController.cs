using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenter.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Kullanıcının randevu alacağı ana ekran (İsterseniz burayı Create'e yönlendirebilirsiniz)
        public IActionResult Index()
        {
            return RedirectToAction("Create");
        }

        // ------------------ RANDEVU OLUSTURMA (GET) ------------------
        public IActionResult Create()
        {
            // Dropdown listeleri için verileri ViewBag'e atıyoruz
            ViewBag.Hizmetler = new SelectList(_context.Hizmetler.ToList(), "Id", "Ad");
            ViewBag.Antrenorler = new SelectList(_context.Antrenorler.ToList(), "Id", "AdSoyad");

            return View();
        }

        // ------------------ RANDEVU OLUSTURMA (POST) ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            // Formdan gelmeyen ama zorunlu alanları dolduralım
            var userId = _userManager.GetUserId(User); // Giriş yapan kullanıcının ID'si
            randevu.UserId = userId;
            randevu.Onaylandi = false; // Varsayılan olarak onaysız

            // Müsaitlik kontrolü (Basit versiyon: Aynı antrenöre aynı saatte randevu var mı?)
            bool musaitDegil = _context.Randevular.Any(x =>
                x.AntrenorId == randevu.AntrenorId &&
                x.Tarih.Date == randevu.Tarih.Date &&
                x.Saat == randevu.Saat);

            if (musaitDegil)
            {
                ModelState.AddModelError("", "Seçtiğiniz antrenör bu tarih ve saatte dolu.");
            }

            if (ModelState.IsValid)
            {
                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRandevus");
            }

            // Hata varsa dropdownları tekrar doldur
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
                .Where(r => r.UserId == userId) // Sadece bu kullanıcının randevuları
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();

            return View(randevular);
        }

        // Randevu İptal Etme (Kullanıcı kendi randevusunu silebilir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            // Sadece kendi randevusunu silebilir
            if (randevu != null && randevu.UserId == userId)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyRandevus");
        }
    }
}