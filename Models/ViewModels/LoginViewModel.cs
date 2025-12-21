// =============================================================================
// DOSYA: LoginViewModel.cs
// AÇIKLAMA: Kullanıcı girişi için view model
// NAMESPACE: Global (FitnessCenter.Models.ViewModels altında kullanılabilir)
// KULLANIM: Account/Login view'ında kullanıcı giriş formu için
// =============================================================================

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Kullanıcı giriş formu için view model.
/// Login sayfasındaki form verilerini toplar ve doğrular.
/// AccountController.Login action'ına gönderilir.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Kullanıcının email adresi.
    /// Giriş için kullanıcı adı olarak email kullanılır.
    /// </summary>
    /// <example>kullanici@example.com</example>
    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    /// <summary>
    /// Kullanıcının şifresi.
    /// Güvenlik için DataType.Password ile maskelenir.
    /// </summary>
    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; }

    /// <summary>
    /// "Beni Hatırla" seçeneği.
    /// true ise persistent cookie oluşturulur (oturum kalıcı olur).
    /// false ise session cookie kullanılır (tarayıcı kapanınca oturum sona erer).
    /// </summary>
    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
