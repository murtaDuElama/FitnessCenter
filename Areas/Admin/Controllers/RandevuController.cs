// =============================================================================
// DOSYA: RandevuController.cs (Admin Area)
// AÇIKLAMA: Admin paneli randevu yönetimi controller'ı
// NAMESPACE: FitnessCenter.Areas.Admin.Controllers
// ALAN: Admin (/Admin/Randevu/...)
// YETKİLENDİRME: Sadece "Admin" rolündeki kullanıcılar erişebilir
// =============================================================================

using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin paneli randevu yönetimi controller sınıfı.
    /// Randevu listeleme, onaylama, iptal etme ve silme işlemlerini yönetir.
    /// Sadece Admin rolündeki kullanıcılar erişebilir.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        /// <summary>Randevu repository - veritabanı işlemleri için</summary>
        private readonly IRandevuRepository _randevuRepository;

        /// <summary>
        /// RandevuController constructor.
        /// </summary>
        /// <param name="randevuRepository">Randevu repository</param>
        public RandevuController(IRandevuRepository randevuRepository)
        {
            _randevuRepository = randevuRepository;
        }

        // ===================== LİSTELEME =====================

        /// <summary>
        /// Randevu listesi sayfası.
        /// GET: /Admin/Randevu
        /// Tüm randevuları hizmet ve antrenör bilgileriyle listeler.
        /// </summary>
        /// <returns>Randevu listesi view'ı</returns>
        public async Task<IActionResult> Index()
        {
            // İlişkili verilerle birlikte getir (Include)
            var randevular = await _randevuRepository.GetAllWithDetailsAsync();
            return View(randevular);
        }

        /// <summary>
        /// Randevu yönetimi sayfası (Index alias).
        /// GET: /Admin/Randevu/Manage
        /// Index ile aynı içeriği gösterir.
        /// </summary>
        /// <returns>Randevu listesi view'ı</returns>
        public async Task<IActionResult> Manage()
        {
            var randevular = await _randevuRepository.GetAllWithDetailsAsync();
            return View("Index", randevular);
        }

        // ===================== RANDEVU ONAYLAMA =====================

        /// <summary>
        /// Randevu onaylama işlemi.
        /// POST: /Admin/Randevu/Onayla
        /// Bekleyen randevuyu onaylar (Onaylandi = true).
        /// </summary>
        /// <param name="id">Onaylanacak randevu ID</param>
        /// <returns>Listeye yönlendirme</returns>
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

            // Onay durumunu güncelle
            randevu.Onaylandi = true;
            await _randevuRepository.UpdateAsync(randevu);

            TempData["Success"] = "Randevu onaylandı!";
            return RedirectToAction(nameof(Index));
        }

        // ===================== RANDEVU İPTAL =====================

        /// <summary>
        /// Randevu iptal etme işlemi.
        /// POST: /Admin/Randevu/IptalEt
        /// Onaylı randevuyu iptal eder (Onaylandi = false).
        /// </summary>
        /// <param name="id">İptal edilecek randevu ID</param>
        /// <returns>Listeye yönlendirme</returns>
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

            // Onay durumunu geri al
            randevu.Onaylandi = false;
            await _randevuRepository.UpdateAsync(randevu);

            TempData["Warning"] = "Randevu iptal edildi!";
            return RedirectToAction(nameof(Index));
        }

        // ===================== RANDEVU SİLME =====================

        /// <summary>
        /// Randevu silme işlemi.
        /// POST: /Admin/Randevu/Sil
        /// Randevuyu veritabanından kalıcı olarak siler.
        /// </summary>
        /// <param name="id">Silinecek randevu ID</param>
        /// <returns>Listeye yönlendirme</returns>
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