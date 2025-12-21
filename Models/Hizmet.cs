// =============================================================================
// DOSYA: Hizmet.cs
// AÇIKLAMA: Hizmet entity modeli - Fitness Center'ın sunduğu hizmetleri tanımlar
// NAMESPACE: Global (FitnessCenter.Models altında kullanılabilir)
// =============================================================================

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Hizmet entity sınıfı.
/// Fitness Center'ın sunduğu fitness hizmetlerini temsil eder.
/// Örnek hizmetler: Fitness Antrenmanı, Yoga Dersi, Pilates, Personal Training vb.
/// </summary>
public class Hizmet
{
    /// <summary>
    /// Hizmetin benzersiz kimlik numarası (Primary Key).
    /// Veritabanı tarafından otomatik oluşturulur.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Hizmetin adı.
    /// Veritabanında unique constraint ile benzersizliği sağlanır.
    /// Örnek: "Fitness Antrenmanı", "Yoga Dersi"
    /// </summary>
    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    public string Ad { get; set; }

    /// <summary>
    /// Hizmetin süresi dakika cinsinden.
    /// Randevu oluştururken bu süre kullanılır.
    /// Örnek: 60 (1 saat), 90 (1.5 saat)
    /// </summary>
    [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
    [Range(15, 180, ErrorMessage = "Süre 15-180 dakika arasında olmalıdır.")]
    public int Sure { get; set; }  // Dakika cinsinden

    /// <summary>
    /// Hizmetin ücreti TL cinsinden.
    /// Decimal tipinde hassas para hesaplaması için kullanılır.
    /// </summary>
    [Required(ErrorMessage = "Hizmet ücreti zorunludur.")]
    [Range(0, 10000, ErrorMessage = "Ücret 0-10000 TL arasında olmalıdır.")]
    public decimal Ucret { get; set; }
}
