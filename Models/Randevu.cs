using System.ComponentModel.DataAnnotations;
using FitnessCenter.Models; // Hizmet, Antrenor, ApplicationUser için gerekli

public class Randevu
{
    public int Id { get; set; }

    // Üyeye ait görüntülenecek adsoyad (isteğe bağlı)
    [Required]
    public string AdSoyad { get; set; }

    // ------------------ İLİŞKİLER ------------------

    [Required]
    public int HizmetId { get; set; }
    public Hizmet Hizmet { get; set; }

    [Required]
    public int AntrenorId { get; set; }
    public Antrenor Antrenor { get; set; }

    // ------------------ TARİH - SAAT ------------------

    [Required]
    public DateTime Tarih { get; set; }

    [Required]
    public string Saat { get; set; }

    // ------------------ ÜYE İLİŞKİSİ ------------------

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }   // ⭐ Eksik olan navigation property

    // ------------------ DURUM ------------------
    public bool Onaylandi { get; set; } = false;
}
