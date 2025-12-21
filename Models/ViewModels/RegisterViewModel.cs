// =============================================================================
// DOSYA: RegisterViewModel.cs
// AÇIKLAMA: Kullanıcı kaydı için view model
// NAMESPACE: FitnessCenter.Models.ViewModels
// KULLANIM: Account/Register view'ında kullanıcı kayıt formu için
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models.ViewModels
{
    /// <summary>
    /// Kullanıcı kayıt formu için view model.
    /// Register sayfasındaki form verilerini toplar ve doğrular.
    /// AccountController.Register action'ına gönderilir.
    /// Başarılı kayıt sonrası ApplicationUser entity'si oluşturulur.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Kullanıcının ad ve soyadı.
        /// ApplicationUser.AdSoyad alanına kaydedilir.
        /// </summary>
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        public string AdSoyad { get; set; }

        /// <summary>
        /// Kullanıcının email adresi.
        /// Hem email hem de kullanıcı adı olarak kullanılır.
        /// Sistemde benzersiz olmalıdır.
        /// </summary>
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Kullanıcının şifresi.
        /// Minimum 3 karakter (test amacıyla düşük tutulmuştur).
        /// </summary>
        /// <remarks>
        /// Üretim ortamında daha güçlü şifre politikası uygulanmalıdır.
        /// </remarks>
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        /// <summary>
        /// Şifre onayı alanı.
        /// Password alanı ile birebir eşleşmeli.
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        [Display(Name = "Şifre Onayı")]
        public string ConfirmPassword { get; set; }
    }
}