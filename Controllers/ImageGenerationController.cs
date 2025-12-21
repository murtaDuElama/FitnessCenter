// =============================================================================
// DOSYA: ImageGenerationController.cs
// AÇIKLAMA: AI destekli egzersiz görseli üretme controller'ı
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /ImageGeneration/Index, /ImageGeneration/Generate
// API: Pollinations.ai (ücretsiz, API key gerektirmez)
// GEREKSİNİM: Giriş yapmış kullanıcılar (Authorize)
// =============================================================================

using FitnessCenter.Models.ViewModels;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// AI görsel üretme controller sınıfı.
    /// Pollinations.ai API'si ile egzersiz görselleri oluşturur.
    /// Kullanıcı seçtiği egzersiz, gün sayısı ve tekrar bilgilerine göre 
    /// kişiselleştirilmiş görsel üretir.
    /// </summary>
    [Authorize]  // Sadece giriş yapmış kullanıcılar erişebilir
    public class ImageGenerationController : Controller
    {
        /// <summary>
        /// Görsel üretme servisi - Pollinations.ai API ile iletişim kurar
        /// </summary>
        private readonly IImageGenerationService _imageService;

        /// <summary>
        /// ImageGenerationController constructor.
        /// Dependency Injection ile IImageGenerationService alır.
        /// </summary>
        /// <param name="imageService">Görsel üretme servis instance'ı</param>
        /// <exception cref="ArgumentNullException">imageService parametresi null ise</exception>
        public ImageGenerationController(IImageGenerationService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        // ===================== ANA SAYFA =====================

        /// <summary>
        /// Görsel üretme sayfası.
        /// GET: /ImageGeneration veya /ImageGeneration/Index
        /// Egzersiz seçimi, gün ve tekrar sayısı girişi için form içerir.
        /// </summary>
        /// <returns>Boş form ile Index view'ı</returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Yeni boş ViewModel ile form göster
            return View(new ImageGenerationViewModel());
        }

        // ===================== GÖRSEL ÜRETME =====================

        /// <summary>
        /// Egzersiz görseli üretme.
        /// POST: /ImageGeneration/Generate
        /// Kullanıcının seçtiği parametrelere göre AI görsel üretir.
        /// </summary>
        /// <param name="model">Egzersiz bilgileri (isim, gün, tekrar)</param>
        /// <returns>Üretilen görsel URL'i ile Index view'ı</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(ImageGenerationViewModel model)
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Pollinations.ai API'den görsel URL'i al
                var imageUrl = await _imageService.GenerateExerciseImageAsync(
                    model.ExerciseName,  // Egzersiz adı (örn: "Squat")
                    model.Days,          // Haftalık gün sayısı (1-7)
                    model.Reps           // Set başına tekrar sayısı
                );

                // Üretilen görsel URL'ini modele ekle
                model.GeneratedImageUrl = imageUrl;
                return View("Index", model);
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıyı bilgilendir
                TempData["Error"] = $"Görsel üretilirken bir hata oluştu: {ex.Message}";
                return View("Index", model);
            }
        }
    }
}
