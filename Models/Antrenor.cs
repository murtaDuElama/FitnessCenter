// =============================================================================
// DOSYA: Antrenor.cs
// AÇIKLAMA: Antrenör entity modeli - Fitness Center'daki eğitmenleri temsil eder
// NAMESPACE: Global (FitnessCenter.Models altında kullanılabilir)
// =============================================================================

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Antrenör entity sınıfı.
/// Fitness Center'da çalışan eğitmenlerin bilgilerini tutar.
/// Randevu sistemi ile ilişkilidir - her randevu bir antrenöre atanır.
/// </summary>
public class Antrenor
{
    /// <summary>
    /// Antrenörün benzersiz kimlik numarası (Primary Key).
    /// Veritabanı tarafından otomatik oluşturulur.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Antrenörün ad ve soyadı.
    /// Zorunlu alan - boş bırakılamaz.
    /// </summary>
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    public string AdSoyad { get; set; }

    /// <summary>
    /// Antrenörün uzmanlık alanı.
    /// Örnek: "Fitness", "Yoga", "Pilates", "Kickbox" vb.
    /// </summary>
    [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
    public string Uzmanlik { get; set; }

    /// <summary>
    /// Antrenörün profil fotoğrafı URL'i.
    /// İsteğe bağlı alan - null olabilir.
    /// </summary>
    public string? FotografUrl { get; set; }

    // ===================== ÇALIŞMA SAATLERİ =====================
    // Antrenörün müsait olduğu saat aralığını belirler.
    // Randevu oluştururken bu saatler kontrol edilir.
    // ============================================================

    /// <summary>
    /// Antrenörün çalışmaya başladığı saat.
    /// Format: "HH:mm" (örn: "09:00")
    /// Varsayılan değer: "09:00"
    /// </summary>
    [Required(ErrorMessage = "Çalışma başlangıç saati zorunludur.")]
    [StringLength(5, ErrorMessage = "Saat formatı HH:mm olmalıdır.")]
    public string CalismaBaslangicSaati { get; set; } = "09:00";

    /// <summary>
    /// Antrenörün çalışmayı bitirdiği saat.
    /// Format: "HH:mm" (örn: "18:00")
    /// Varsayılan değer: "15:00"
    /// </summary>
    [Required(ErrorMessage = "Çalışma bitiş saati zorunludur.")]
    [StringLength(5, ErrorMessage = "Saat formatı HH:mm olmalıdır.")]
    public string CalismaBitisSaati { get; set; } = "15:00";
}
