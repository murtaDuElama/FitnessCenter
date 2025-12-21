// =============================================================================
// DOSYA: HizmetController.cs
// AÇIKLAMA: Public hizmet listeleme controller'ı
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /Hizmet/Index
// NOT: CRUD işlemleri Admin alanındaki HizmetController'da yapılır
// =============================================================================

using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// Public hizmet controller sınıfı.
    /// Ziyaretçilere sunulan hizmetlerin listesini gösterir.
    /// CRUD işlemleri Admin alanında yönetilir.
    /// </summary>
    public class HizmetController : Controller
    {
        /// <summary>
        /// Hizmet repository - veritabanı işlemleri için
        /// </summary>
        private readonly IHizmetRepository _hizmetRepository;

        /// <summary>
        /// HizmetController constructor.
        /// Dependency Injection ile repository alır.
        /// </summary>
        /// <param name="hizmetRepository">Hizmet repository instance'ı</param>
        public HizmetController(IHizmetRepository hizmetRepository)
        {
            _hizmetRepository = hizmetRepository;
        }

        /// <summary>
        /// Hizmet listesi sayfası.
        /// GET: /Hizmet veya /Hizmet/Index
        /// Tüm hizmetleri fiyat ve süre bilgileriyle listeler.
        /// </summary>
        /// <returns>Hizmet listesi view'ı</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Tüm hizmetleri repository'den al
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }
    }
}
