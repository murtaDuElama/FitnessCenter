using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FitnessCenter.Models;
using FitnessCenter.Repositories;
using FitnessCenter.Services;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class RandevuController : Controller
    {
        private readonly IHizmetRepository _hizmetRepository;
        private readonly IAntrenorRepository _antrenorRepository;
        private readonly RandevuService _randevuService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(
            IHizmetRepository hizmetRepository,
            IAntrenorRepository antrenorRepository,
            RandevuService randevuService,
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
        public async Task<IActionResult> SelectTime(int hizmetId, int antrenorId)
        {
            // Tanımlı saat aralıkları + dolu saat kontrolü artık servis içinde
            var musait = await _randevuService.GetMusaitSaatlerAsync(antrenorId, DateTime.Today);

            ViewBag.HizmetId = hizmetId;
            ViewBag.AntrenorId = antrenorId;

            return View(musait);
        }

        // ------------------ 4) RANDEVU OLUŞTUR ------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int hizmetId, int antrenorId, string saat)
        {
            // Yapıyı bozmayalım: basit kontrol burada kalsın
            if (string.IsNullOrWhiteSpace(saat))
            {
                TempData["Error"] = "Lütfen bir saat seçiniz.";
                return RedirectToAction("SelectTime", new { hizmetId, antrenorId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var (success, error, _) = await _randevuService.CreateAsync(
                user,
                hizmetId,
                antrenorId,
                DateTime.Today,
                saat);

            if (!success)
            {
                TempData["Error"] = error ?? "Randevu oluşturulamadı. Lütfen tekrar deneyin.";
                return RedirectToAction("SelectTime", new { hizmetId, antrenorId });
            }

            TempData["Success"] = "Randevunuz başarıyla oluşturuldu.";
            return RedirectToAction("MyRandevus");
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
