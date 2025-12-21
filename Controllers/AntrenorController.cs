// =============================================================================
// DOSYA: AntrenorController.cs
// AÇIKLAMA: Public antrenör listeleme controller'ı
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /Antrenor/Index
// NOT: CRUD işlemleri Admin alanındaki AntrenorController'da yapılır
// =============================================================================

using FitnessCenter.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// Public antrenör controller sınıfı.
    /// Ziyaretçilere antrenör listesini gösterir.
    /// CRUD işlemleri Admin alanında yönetilir.
    /// </summary>
    public class AntrenorController : Controller
    {
        /// <summary>
        /// Antrenör repository - veritabanı işlemleri için
        /// </summary>
        private readonly IAntrenorRepository _antrenorRepository;

        /// <summary>
        /// AntrenorController constructor.
        /// Dependency Injection ile repository alır.
        /// </summary>
        /// <param name="antrenorRepository">Antrenör repository instance'ı</param>
        public AntrenorController(IAntrenorRepository antrenorRepository)
        {
            _antrenorRepository = antrenorRepository;
        }

        /// <summary>
        /// Antrenör listesi sayfası.
        /// GET: /Antrenor veya /Antrenor/Index
        /// Tüm antrenörleri kart görünümünde listeler.
        /// </summary>
        /// <returns>Antrenör listesi view'ı</returns>
        public async Task<IActionResult> Index()
        {
            // Tüm antrenörleri repository'den al
            var antrenorler = await _antrenorRepository.GetAllAsync();
            return View(antrenorler);
        }
    }
}
