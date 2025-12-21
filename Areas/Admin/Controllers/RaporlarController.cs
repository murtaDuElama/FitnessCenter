// =============================================================================
// DOSYA: RaporlarController.cs (Admin Area)
// AÇIKLAMA: Admin paneli raporlar sayfası controller'ı
// NAMESPACE: FitnessCenter.Areas.Admin.Controllers
// ALAN: Admin (/Admin/Raporlar/...)
// YETKİLENDİRME: Sadece "Admin" rolündeki kullanıcılar erişebilir
// NOT: Detaylı raporlar API üzerinden çekilir (RaporController API)
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin paneli raporlar controller sınıfı.
    /// Raporlama sayfasını gösterir.
    /// Detaylı veriler API endpoint'lerinden AJAX ile çekilir.
    /// </summary>
    /// <remarks>
    /// İlişkili API endpoint'leri:
    /// - GET /api/Rapor/istatistikler - Genel istatistikler
    /// - GET /api/Rapor/randevular - Randevu listesi
    /// - GET /api/Rapor/gelir - Gelir raporu
    /// - GET /api/Rapor/antrenor/{id} - Antrenör detay raporu
    /// </remarks>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RaporlarController : Controller
    {
        /// <summary>
        /// Raporlar ana sayfası.
        /// GET: /Admin/Raporlar
        /// Grafik ve istatistik dashboard'u gösterir.
        /// Veriler JavaScript ile API'den çekilir.
        /// </summary>
        /// <returns>Raporlar view'ı</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}