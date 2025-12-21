

using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// Ana sayfa controller sınıfı.
    /// Uygulamanın temel sayfalarını (Ana Sayfa, Gizlilik, Hata) yönetir.
    /// Admin kullanıcıları otomatik olarak Admin paneline yönlendirilir.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger servisi - hata ve bilgi loglaması için
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// HomeController constructor.
        /// Dependency Injection ile ILogger servisi alır.
        /// </summary>
        /// <param name="logger">Loglama servisi</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Ana sayfa action'ı.
        /// GET: / veya /Home/Index
        /// Admin kullanıcıları Admin paneline yönlendirilir.
        /// </summary>
        /// <returns>Ana sayfa view'ı veya Admin yönlendirmesi</returns>
        public IActionResult Index()
        {
            // Admin kontrolü: Admin rolündeki kullanıcıları Admin paneline yönlendir
            if (User.Identity != null &&
                User.Identity.IsAuthenticated &&
                User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View();
        }

        /// <summary>
        /// Gizlilik politikası sayfası.
        /// GET: /Home/Privacy
        /// </summary>
        /// <returns>Privacy view'ı</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Hata sayfası action'ı.
        /// Uygulama genelindeki hatalar buraya yönlendirilir.
        /// ResponseCache devre dışı - hatalar cache'lenmez.
        /// </summary>
        /// <returns>Error view'ı ile ErrorViewModel</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // RequestId: Mevcut Activity ID veya HTTP TraceIdentifier
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
