// =============================================================================
// DOSYA: AccountController.cs
// AÇIKLAMA: Kullanıcı kimlik doğrulama controller'ı - Giriş, Kayıt, Çıkış işlemleri
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /Account/Login, /Account/Register, /Account/Logout, /Account/DeleteAccount
// =============================================================================

using FitnessCenter.Models;
using FitnessCenter.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// Kullanıcı hesap yönetimi controller sınıfı.
    /// ASP.NET Identity ile entegre çalışır.
    /// Kayıt, giriş, çıkış ve hesap silme işlemlerini yönetir.
    /// </summary>
    public class AccountController : Controller
    {
        // ===================== DEPENDENCY INJECTION =====================

        /// <summary>
        /// UserManager servisi - kullanıcı CRUD işlemleri için
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// SignInManager servisi - oturum yönetimi için
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// AccountController constructor.
        /// Identity servislerini DI ile alır.
        /// </summary>
        /// <param name="userManager">Kullanıcı yönetim servisi</param>
        /// <param name="signInManager">Oturum yönetim servisi</param>
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===================== KAYIT İŞLEMLERİ =====================

        /// <summary>
        /// Kayıt formu sayfası.
        /// GET: /Account/Register
        /// </summary>
        /// <returns>Kayıt formu view'ı</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Yeni kullanıcı kaydı işlemi.
        /// POST: /Account/Register
        /// Başarılı kayıtta otomatik giriş yapılır ve "Uye" rolü atanır.
        /// </summary>
        /// <param name="model">Kayıt form verileri</param>
        /// <returns>Ana sayfaya yönlendirme veya hatalı form</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
                return View(model);

            // Yeni ApplicationUser oluştur
            var user = new ApplicationUser
            {
                AdSoyad = model.AdSoyad,
                Email = model.Email,
                UserName = model.Email  // Email'i kullanıcı adı olarak kullan
            };

            // Identity ile kullanıcı oluştur
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Varsayılan "Uye" rolünü ata
                await _userManager.AddToRoleAsync(user, "Uye");

                // Otomatik giriş yap
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            // Hataları ModelState'e ekle
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ===================== GİRİŞ İŞLEMLERİ =====================

        /// <summary>
        /// Giriş formu sayfası.
        /// GET: /Account/Login
        /// </summary>
        /// <returns>Giriş formu view'ı</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Kullanıcı giriş işlemi.
        /// POST: /Account/Login
        /// Admin kullanıcıları Admin paneline yönlendirilir.
        /// </summary>
        /// <param name="model">Giriş form verileri</param>
        /// <param name="returnUrl">Giriş sonrası yönlendirilecek URL (opsiyonel)</param>
        /// <returns>Yönlendirme veya hatalı form</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Email ve şifre ile giriş dene
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false  // Başarısız denemede hesap kilitlenmez
                );

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Admin kullanıcıyı Admin paneline yönlendir
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }

                    // ReturnUrl varsa ve güvenliyse oraya yönlendir
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Normal kullanıcıyı ana sayfaya yönlendir
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            }

            return View(model);
        }

        // ===================== ÇIKIŞ İŞLEMİ =====================

        /// <summary>
        /// Kullanıcı çıkış işlemi.
        /// GET: /Account/Logout
        /// Oturumu sonlandırır ve giriş sayfasına yönlendirir.
        /// </summary>
        /// <returns>Giriş sayfasına yönlendirme</returns>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ===================== HESAP SİLME =====================

        /// <summary>
        /// Kullanıcı hesabını silme işlemi.
        /// POST: /Account/DeleteAccount
        /// Mevcut kullanıcının hesabını kalıcı olarak siler.
        /// </summary>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            // Mevcut kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Kullanıcıyı veritabanından sil
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Oturumu kapat ve ana sayfaya yönlendir
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }

                // Silme hataları (örn: ilişkili veriler)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}