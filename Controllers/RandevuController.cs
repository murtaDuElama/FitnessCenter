using FitnessCenter.Models;
using FitnessCenter.Repositories;
using FitnessCenter.Services; // sende farklıysa düzelt
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    public class RandevuController : Controller
    {
        private readonly IHizmetRepository _hizmetRepository;
        private readonly IAntrenorRepository _antrenorRepository;
        private readonly IRandevuService _randevuService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(
            IHizmetRepository hizmetRepository,
            IAntrenorRepository antrenorRepository,
            IRandevuService randevuService,
            UserManager<ApplicationUser> userManager)
        {
            _hizmetRepository = hizmetRepository;
            _antrenorRepository = antrenorRepository;
            _randevuService = randevuService;
            _userManager = userManager;
        }

        // ------------------ 1) HİZMET SEÇ ------------------
        [HttpGet]
        public async Task<IActionResult> SelectService()
        {
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }

        // ------------------ 2) HİZMETE GÖRE ANTRENÖR SEÇ ------------------
        [HttpGet]
        public async Task<IActionResult> SelectTrainer(int hizmetId)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(hizmetId);

            if (hizmet == null)
                return NotFound();

            // Uzmanlık alanına göre eğitmenleri filtrele
            var antrenorler = await _antrenorRepository.GetByUzmanlikAsync(hizmet.Ad);

            ViewBag.Hizmet = hizmet;
            return View(antrenorler);
        }

        // ------------------ 3) ANTRENÖR MÜSAİTLİK SAATLERİ ------------------
        [HttpGet]
        public async Task<IActionResult> SelectTime(int hizmetId, int antrenorId, DateTime? tarih)
        {
            var bugun = DateTime.Today;
            var seciliTarih = (tarih?.Date) ?? bugun;

            if (seciliTarih < bugun)
            {
                TempData["Error"] = "Geçmiş tarihler için randevu oluşturulamaz. Bugün veya ileri bir tarihi seçiniz.";
                seciliTarih = bugun;
            }

            // Tanımlı saat aralıkları + dolu saat kontrolü servis içinde
            var musait = await _randevuService.GetMusaitSaatlerAsync(antrenorId, seciliTarih);

            ViewBag.HizmetId = hizmetId;
            ViewBag.AntrenorId = antrenorId;
            ViewBag.SeciliTarih = seciliTarih.ToString("yyyy-MM-dd");
            ViewBag.SeciliTarihLabel = seciliTarih.ToString("dd.MM.yyyy");

            return View(musait);
        }

        // ------------------ 4) RANDEVU OLUŞTUR ------------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int hizmetId, int antrenorId, DateTime tarih, string saat)
        {
            if (tarih == default)
            {
                TempData["Error"] = "Lütfen bir tarih seçiniz.";
                return RedirectToAction(nameof(SelectTime), new { hizmetId, antrenorId });
            }

            // Yapıyı bozmayalım: basit kontrol burada kalsın
            if (string.IsNullOrWhiteSpace(saat))
            {
                TempData["Error"] = "Lütfen bir saat seçiniz.";
                return RedirectToAction(nameof(SelectTime), new
                {
                    hizmetId,
                    antrenorId,
                    tarih = tarih.ToString("yyyy-MM-dd")
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var (success, error, _) = await _randevuService.CreateAsync(
                user,
                hizmetId,
                antrenorId,
                tarih,
                saat);

            if (!success)
            {
                TempData["Error"] = error ?? "Randevu oluşturulamadı. Lütfen tekrar deneyin.";
                return RedirectToAction(nameof(SelectTime), new
                {
                    hizmetId,
                    antrenorId,
                    tarih = tarih.ToString("yyyy-MM-dd")
                });
            }

            TempData["Success"] = "Randevunuz başarıyla oluşturuldu.";
            return RedirectToAction(nameof(MyRandevus));
        }

        // ------------------ 5) RANDEVULARIM ------------------
        [HttpGet]
        public async Task<IActionResult> MyRandevus()
        {
            var userId = _userManager.GetUserId(User);
            var randevular = await _randevuService.GetUserRandevularAsync(userId);
            return View(randevular);
        }

        // ------------------ 6) RANDEVU İPTAL ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _randevuService.IptalEtAsync(id, userId);

            if (success)
            {
                TempData["Success"] = "Randevu iptal edildi.";
            }
            else
            {
                TempData["Error"] = "Randevu iptal edilemedi.";
            }

            return RedirectToAction(nameof(MyRandevus));
        }
    }
}
