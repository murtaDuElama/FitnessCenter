using System.ComponentModel.DataAnnotations;

public class Antrenor
{
    public int Id { get; set; }

    [Required]
    public string AdSoyad { get; set; }

    [Required]
    public string Uzmanlik { get; set; }

    public string? FotografUrl { get; set; }  // Opsiyonel
}
