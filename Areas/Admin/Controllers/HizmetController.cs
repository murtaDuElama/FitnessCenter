// =============================================================================
// DOSYA: HizmetController.cs (Admin Area)
// AÇIKLAMA: Admin paneli hizmet CRUD işlemleri controller'ı
// NAMESPACE: FitnessCenter.Areas.Admin.Controllers
// ALAN: Admin (/Admin/Hizmet/...)
// YETKİLENDİRME: Sadece "Admin" rolündeki kullanıcılar erişebilir
// =============================================================================

using FitnessCenter.Models;
using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin paneli hizmet yönetimi controller sınıfı.
    /// Hizmet CRUD (Create, Read, Update, Delete) işlemlerini yönetir.
    /// Sadece Admin rolündeki kullanıcılar erişebilir.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        /// <summary>Hizmet repository - veritabanı işlemleri için</summary>
        private readonly IHizmetRepository _hizmetRepository;

        /// <summary>
        /// HizmetController constructor.
        /// </summary>
        /// <param name="hizmetRepository">Hizmet repository</param>
        public HizmetController(IHizmetRepository hizmetRepository)
        {
            _hizmetRepository = hizmetRepository;
        }

        // ===================== LİSTELEME =====================

        /// <summary>
        /// Hizmet listesi sayfası.
        /// GET: /Admin/Hizmet
        /// Tüm hizmetleri tablo formatında listeler.
        /// </summary>
        /// <returns>Hizmet listesi view'ı</returns>
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }

        // ===================== DETAY GÖRÜNTÜLEME =====================

        /// <summary>
        /// Hizmet detay sayfası.
        /// GET: /Admin/Hizmet/Details/{id}
        /// Seçilen hizmetin tüm bilgilerini gösterir.
        /// </summary>
        /// <param name="id">Hizmet ID</param>
        /// <returns>Detay view'ı veya 404</returns>
        public async Task<IActionResult> Details(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // ===================== YENİ HİZMET OLUŞTURMA =====================

        /// <summary>
        /// Yeni hizmet oluşturma formu.
        /// GET: /Admin/Hizmet/Create
        /// Boş form gösterir.
        /// </summary>
        /// <returns>Oluşturma formu view'ı</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Yeni hizmet kaydetme işlemi.
        /// POST: /Admin/Hizmet/Create
        /// Form verilerini doğrular ve veritabanına kaydeder.
        /// </summary>
        /// <param name="h">Hizmet model verisi</param>
        /// <returns>Başarıda listeye, hatada forma yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet h)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
                return View(h);

            // Veritabanına kaydet
            await _hizmetRepository.AddAsync(h);

            TempData["Success"] = "Hizmet başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // ===================== HİZMET DÜZENLEME =====================

        /// <summary>
        /// Hizmet düzenleme formu.
        /// GET: /Admin/Hizmet/Edit/{id}
        /// Mevcut verileri form'a doldurur.
        /// </summary>
        /// <param name="id">Düzenlenecek hizmet ID</param>
        /// <returns>Düzenleme formu view'ı veya 404</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        /// <summary>
        /// Hizmet güncelleme işlemi.
        /// POST: /Admin/Hizmet/Edit
        /// Değişiklikleri doğrular ve veritabanına kaydeder.
        /// </summary>
        /// <param name="h">Güncellenmiş hizmet verisi</param>
        /// <returns>Başarıda listeye, hatada forma yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Hizmet h)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
                return View(h);

            // Mevcut kaydı bul
            var mevcut = await _hizmetRepository.GetByIdAsync(h.Id);
            if (mevcut == null)
                return NotFound();

            // EF Core tracking sorununu önlemek için mevcut entity'yi güncelle
            mevcut.Ad = h.Ad;
            mevcut.Ucret = h.Ucret;
            mevcut.Sure = h.Sure;

            await _hizmetRepository.UpdateAsync(mevcut);

            TempData["Success"] = "Hizmet başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // ===================== HİZMET SİLME =====================

        /// <summary>
        /// Hizmet silme onay sayfası.
        /// GET: /Admin/Hizmet/Delete/{id}
        /// Silme işleminden önce onay ekranı gösterir.
        /// </summary>
        /// <param name="id">Silinecek hizmet ID</param>
        /// <returns>Silme onay view'ı veya 404</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        /// <summary>
        /// Hizmet silme işlemi.
        /// POST: /Admin/Hizmet/Delete/{id}
        /// Onay sonrası hizmeti veritabanından siler.
        /// </summary>
        /// <param name="id">Silinecek hizmet ID</param>
        /// <returns>Listeye yönlendirme</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _hizmetRepository.GetByIdAsync(id);
            if (hizmet == null)
                return NotFound();

            await _hizmetRepository.RemoveAsync(hizmet);

            TempData["Delete"] = "Hizmet başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}