// =============================================================================
// DOSYA: Randevu.cs
// AÇIKLAMA: Randevu entity modeli - Üyelerin antrenörlerle olan randevularını yönetir
// NAMESPACE: Global (FitnessCenter.Models altında kullanılabilir)
// İLİŞKİLER: Hizmet, Antrenor, ApplicationUser ile Foreign Key ilişkisi
// =============================================================================

using FitnessCenter.Models; // Hizmet, Antrenor, ApplicationUser için gerekli
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Randevu entity sınıfı.
/// Fitness Center'da üyelerin antrenörlerle yaptığı randevuları temsil eder.
/// Her randevu bir hizmete, bir antrenöre ve bir üyeye bağlıdır.
/// </summary>
public class Randevu
{
    /// <summary>
    /// Randevunun benzersiz kimlik numarası (Primary Key).
    /// Veritabanı tarafından otomatik oluşturulur.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Randevu sahibinin ad ve soyadı.
    /// Kullanıcı bilgilerinden otomatik doldurulabilir.
    /// </summary>
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    public string AdSoyad { get; set; }

    // ===================== HİZMET İLİŞKİSİ =====================
    // Her randevu bir hizmete bağlıdır (örn: Fitness, Yoga)
    // ============================================================

    /// <summary>
    /// İlişkili hizmetin ID'si (Foreign Key).
    /// Hizmetler tablosu ile ilişki kurar.
    /// </summary>
    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    public int HizmetId { get; set; }

    /// <summary>
    /// İlişkili Hizmet nesnesi (Navigation Property).
    /// Entity Framework tarafından otomatik doldurulur.
    /// </summary>
    public Hizmet Hizmet { get; set; }

    // ===================== ANTRENÖR İLİŞKİSİ =====================
    // Her randevu bir antrenöre atanır
    // =============================================================

    /// <summary>
    /// İlişkili antrenörün ID'si (Foreign Key).
    /// Antrenorler tablosu ile ilişki kurar.
    /// </summary>
    [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
    public int AntrenorId { get; set; }

    /// <summary>
    /// İlişkili Antrenör nesnesi (Navigation Property).
    /// Entity Framework tarafından otomatik doldurulur.
    /// </summary>
    public Antrenor Antrenor { get; set; }

    // ===================== TARİH VE SAAT =====================
    // Randevunun gerçekleşeceği tarih ve saat bilgisi
    // =========================================================

    /// <summary>
    /// Randevunun tarihi.
    /// Sadece tarih kısmı kullanılır, saat ayrı alanda tutulur.
    /// </summary>
    [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime Tarih { get; set; }

    /// <summary>
    /// Randevunun saati.
    /// Format: "HH:mm" (örn: "10:00", "14:30")
    /// </summary>
    [Required(ErrorMessage = "Randevu saati zorunludur.")]
    public string Saat { get; set; }

    // ===================== ÜYE İLİŞKİSİ =====================
    // Randevuyu oluşturan üye bilgisi (Identity sistemi ile entegre)
    // ============================================================

    /// <summary>
    /// Randevuyu oluşturan kullanıcının ID'si (Foreign Key).
    /// ASP.NET Identity AspNetUsers tablosu ile ilişki kurar.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// İlişkili ApplicationUser nesnesi (Navigation Property).
    /// Üye bilgilerine erişim sağlar.
    /// </summary>
    public ApplicationUser User { get; set; }

    // ===================== DURUM BİLGİSİ =====================
    // Randevunun onay durumu
    // =========================================================

    /// <summary>
    /// Randevunun onay durumu.
    /// true: Admin tarafından onaylandı
    /// false: Henüz onaylanmadı (varsayılan)
    /// </summary>
    public bool Onaylandi { get; set; } = false;
}
