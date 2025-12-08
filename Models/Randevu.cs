using System.ComponentModel.DataAnnotations;

public class Randevu
{
    public int Id { get; set; }

    [Required]
    public string AdSoyad { get; set; }

    [Required]
    public string Telefon { get; set; }

    // İlişkiler
    [Required]
    public int HizmetId { get; set; }
    public Hizmet Hizmet { get; set; }

    [Required]
    public int AntrenorId { get; set; }
    public Antrenor Antrenor { get; set; }

    [Required]
    public DateTime Tarih { get; set; }

    [Required]
    public string Saat { get; set; }

    // ⭐ EKLEDİĞİMİZ TEK ŞEY BU
    public string UserId { get; set; }
}
