// =============================================================================
// DOSYA: ApplicationUser.cs
// AÇIKLAMA: Özelleştirilmiş kullanıcı modeli - ASP.NET Identity ile entegre
// NAMESPACE: FitnessCenter.Models
// KALITIM: IdentityUser sınıfından türetilmiştir
// =============================================================================

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    /// <summary>
    /// Özelleştirilmiş kullanıcı sınıfı.
    /// ASP.NET Identity'nin IdentityUser sınıfından türetilmiştir.
    /// Fitness Center'a özgü ek kullanıcı bilgilerini barındırır.
    /// </summary>
    /// <remarks>
    /// Varsayılan IdentityUser özellikleri (Email, UserName, PasswordHash vb.) 
    /// bu sınıftan miras alınır. Ek olarak Fitness Center'a özgü 
    /// AdSoyad gibi alanlar eklenmiştir.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Kullanıcının ad ve soyadı.
        /// Kayıt sırasında kullanıcıdan alınır.
        /// </summary>
        /// <example>Ahmet Yılmaz</example>
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string? AdSoyad { get; set; }
    }
}