// =============================================================================
// DOSYA: AntrenorController.cs (Admin Area)
// AÇIKLAMA: Admin paneli antrenör CRUD işlemleri controller'ı
// NAMESPACE: FitnessCenter.Areas.Admin.Controllers
// ALAN: Admin (/Admin/Antrenor/...)
// YETKİLENDİRME: Sadece "Admin" rolündeki kullanıcılar erişebilir
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitnessCenter.Models;
using FitnessCenter.Repositories;

namespace FitnessCenter.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin paneli antrenör yönetimi controller sınıfı.
    /// Antrenör CRUD (Create, Read, Update, Delete) işlemlerini yönetir.
    /// Sadece Admin rolündeki kullanıcılar erişebilir.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        /// <summary>Antrenör repository - veritabanı işlemleri için</summary>
        private readonly IAntrenorRepository _antrenorRepository;

        /// <summary>Hizmet repository - uzmanlık alanı listesi için</summary>
        private readonly IHizmetRepository _hizmetRepository;

        /// <summary>
        /// AntrenorController constructor.
        /// </summary>
        /// <param name="antrenorRepository">Antrenör repository</param>
        /// <param name="hizmetRepository">Hizmet repository</param>
        public AntrenorController(
            IAntrenorRepository antrenorRepository,
            IHizmetRepository hizmetRepository)
        {
            _antrenorRepository = antrenorRepository;
            _hizmetRepository = hizmetRepository;
        }

        // ===================== LİSTELEME =====================

        /// <summary>
        /// Antrenör listesi sayfası.
        /// GET: /Admin/Antrenor
        /// Tüm antrenörleri tablo formatında listeler.
        /// </summary>
        /// <returns>Antrenör listesi view'ı</returns>
        public async Task<IActionResult> Index()
        {
            var liste = await _antrenorRepository.GetAllAsync();
            return View(liste);
        }

        // ===================== DETAY GÖRÜNTÜLEME =====================

        /// <summary>
        /// Antrenör detay sayfası.
        /// GET: /Admin/Antrenor/Details/{id}
        /// Seçilen antrenörün tüm bilgilerini gösterir.
        /// </summary>
        /// <param name="id">Antrenör ID</param>
        /// <returns>Detay view'ı veya 404</returns>
        public async Task<IActionResult> Details(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // ===================== YENİ ANTRENÖR OLUŞTURMA =====================

        /// <summary>
        /// Yeni antrenör oluşturma formu.
        /// GET: /Admin/Antrenor/Create
        /// Boş form ve uzmanlık alanları dropdown'ı gösterir.
        /// </summary>
        /// <returns>Oluşturma formu view'ı</returns>
        public async Task<IActionResult> Create()
        {
            // Hizmet adlarını dropdown için al (uzmanlık alanı olarak kullanılır)
            var hizmetler = await _hizmetRepository.GetAllAsync();
            ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();

            return View();
        }

        /// <summary>
        /// Yeni antrenör kaydetme işlemi.
        /// POST: /Admin/Antrenor/Create
        /// Form verilerini doğrular ve veritabanına kaydeder.
        /// </summary>
        /// <param name="a">Antrenör model verisi</param>
        /// <returns>Başarıda listeye, hatada forma yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Antrenor a)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                // Hata durumunda dropdown'ı tekrar doldur
                var hizmetler = await _hizmetRepository.GetAllAsync();
                ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();
                return View(a);
            }

            // Veritabanına kaydet
            await _antrenorRepository.AddAsync(a);

            TempData["Success"] = "Antrenör başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // ===================== ANTRENÖR DÜZENLEME =====================

        /// <summary>
        /// Antrenör düzenleme formu.
        /// GET: /Admin/Antrenor/Edit/{id}
        /// Mevcut verileri form'a doldurur.
        /// </summary>
        /// <param name="id">Düzenlenecek antrenör ID</param>
        /// <returns>Düzenleme formu view'ı veya 404</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            // Dropdown için hizmetleri al
            var hizmetler = await _hizmetRepository.GetAllAsync();
            ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();

            return View(ant);
        }

        /// <summary>
        /// Antrenör güncelleme işlemi.
        /// POST: /Admin/Antrenor/Edit
        /// Değişiklikleri doğrular ve veritabanına kaydeder.
        /// </summary>
        /// <param name="a">Güncellenmiş antrenör verisi</param>
        /// <returns>Başarıda listeye, hatada forma yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Antrenor a)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                var hizmetler = await _hizmetRepository.GetAllAsync();
                ViewBag.Hizmetler = hizmetler.Select(h => h.Ad).ToList();
                return View(a);
            }

            // Mevcut kaydı bul
            var mevcut = await _antrenorRepository.GetByIdAsync(a.Id);
            if (mevcut == null)
                return NotFound();

            // EF Core tracking sorununu önlemek için mevcut entity'yi güncelle
            mevcut.AdSoyad = a.AdSoyad;
            mevcut.Uzmanlik = a.Uzmanlik;
            mevcut.FotografUrl = a.FotografUrl;

            await _antrenorRepository.UpdateAsync(mevcut);

            TempData["Success"] = "Antrenör başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // ===================== ANTRENÖR SİLME =====================

        /// <summary>
        /// Antrenör silme onay sayfası.
        /// GET: /Admin/Antrenor/Delete/{id}
        /// Silme işleminden önce onay ekranı gösterir.
        /// </summary>
        /// <param name="id">Silinecek antrenör ID</param>
        /// <returns>Silme onay view'ı veya 404</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        /// <summary>
        /// Antrenör silme işlemi.
        /// POST: /Admin/Antrenor/Delete/{id}
        /// Onay sonrası antrenörü veritabanından siler.
        /// </summary>
        /// <param name="id">Silinecek antrenör ID</param>
        /// <returns>Listeye yönlendirme</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ant = await _antrenorRepository.GetByIdAsync(id);
            if (ant == null)
                return NotFound();

            await _antrenorRepository.RemoveAsync(ant);

            TempData["Delete"] = "Antrenör başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}