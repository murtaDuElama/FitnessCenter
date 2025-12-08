using System.ComponentModel.DataAnnotations;

public class Hizmet
{
    public int Id { get; set; }

    [Required]
    public string Ad { get; set; }

    [Required]
    public int Sure { get; set; }  // Dakika

    [Required]
    public decimal Ucret { get; set; }
}
