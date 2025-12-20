using System.ComponentModel.DataAnnotations;

public class Antrenor
{
    public int Id { get; set; }

    [Required]
    public string AdSoyad { get; set; }

    [Required]
    public string Uzmanlik { get; set; }

    public string? FotografUrl { get; set; }

    // Antrenörün müsaitlik saat aralığı
    [Required]
    [StringLength(5)]
    public string CalismaBaslangicSaati { get; set; } = "09:00";

    [Required]
    [StringLength(5)]
    public string CalismaBitisSaati { get; set; } = "15:00";
}
