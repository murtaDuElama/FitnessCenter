using Microsoft.AspNetCore.Identity;

namespace FitnessCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        // İstersen özel alanlar ekleyebilirsin:
        // public string AdSoyad { get; set; }
        // public DateTime KayitTarihi { get; set; } = DateTime.Now;
    }
}
