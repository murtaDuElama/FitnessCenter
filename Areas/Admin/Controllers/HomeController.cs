// =============================================================================
// DOSYA: HomeController.cs (Admin Area)
// AÇIKLAMA: Admin paneli ana sayfa controller'ı (Dashboard)
// NAMESPACE: FitnessCenter.Areas.Admin.Controllers
// ALAN: Admin (/Admin/Home/...)
// YETKİLENDİRME: Sadece "Admin" rolündeki kullanıcılar erişebilir
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin paneli ana sayfa controller sınıfı.
    /// Admin kullanıcıları giriş yaptığında bu sayfaya yönlendirilir.
    /// Dashboard istatistikleri ve hızlı erişim linkleri gösterir.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Admin dashboard sayfası.
        /// GET: /Admin veya /Admin/Home/Index
        /// Genel istatistikler ve son kayıtları gösterir.
        /// </summary>
        /// <returns>Dashboard view'ı</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
