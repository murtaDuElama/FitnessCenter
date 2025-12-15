using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly IRandevuRepository _randevuRepository;

        public RandevuController(IRandevuRepository randevuRepository)
        {
            _randevuRepository = randevuRepository;
        }

        // --------------------- RANDEVU LİSTESİ ---------------------
        public async Task<IActionResult> Index()
        {
            var randevular = await _randevuRepository.GetAllWithDetailsAsync();
            return View(randevular);
        }

        // --------------------- ONAYLA ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _randevuRepository.GetByIdAsync(id);
            if (randevu == null)
            {
                TempData["Delete"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            randevu.Onaylandi = true;
            await _randevuRepository.UpdateAsync(randevu);

            TempData["Success"] = "Randevu onaylandı!";
            return RedirectToAction(nameof(Index));
        }

        // --------------------- İPTAL ET ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _randevuRepository.GetByIdAsync(id);
            if (randevu == null)
            {
                TempData["Delete"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            randevu.Onaylandi = false;
            await _randevuRepository.UpdateAsync(randevu);

            TempData["Delete"] = "Randevu iptal edildi!";
            return RedirectToAction(nameof(Index));
        }

        // --------------------- SİL ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _randevuRepository.GetByIdAsync(id);
            if (randevu == null)
            {
                TempData["Delete"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await _randevuRepository.RemoveAsync(randevu);

            TempData["Delete"] = "Randevu başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }
    }
}
