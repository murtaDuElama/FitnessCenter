using FitnessCenter.Models;
using FitnessCenter.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                AdSoyad = model.AdSoyad, // Düzeltildi: FullName -> AdSoyad
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // ⭐ YENİ EKLEME: Kayıt olan herkese varsayılan "Uye" rolü verelim
                await _userManager.AddToRoleAsync(user, "Uye");

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                // Giriş yapan kullanıcıyı bul
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Kullanıcının "Admin" rolü olup olmadığını kontrol et
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Admin ise -> Admin Paneline git
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                // Normal kullanıcı ise -> Anasayfaya git
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Geçersiz email veya şifre.");
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            // Şu anki kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Kullanıcıyı veritabanından sil
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Oturumu kapat
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }

                // Hata olursa (örneğin ilişkili veriler yüzünden)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}