using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class ApplicationUser : IdentityUser
    {
        // SeedData.cs ve diğer yerlerde "AdSoyad" kullandığımız için burayı değiştiriyoruz
        [StringLength(100)]
        public string? AdSoyad { get; set; }
    }
}