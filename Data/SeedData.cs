using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Data
{
    public static class SeedData
    {
        // Metodu asenkron yaptık ve parametre olarak IApplicationBuilder aldık
        public static async Task Seed(IApplicationBuilder app)
        {
            // Servis havuzundan (Scope) gerekli araçları çağırıyoruz
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // 1. Veritabanı Bağlantısı
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 2. Kullanıcı ve Rol Yöneticileri
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Veritabanını güncelle (Migration varsa uygula)
                context.Database.Migrate();

                // ---------------------------------------------------------
                // A. KİMLİK (IDENTITY) VERİLERİ (Roller ve Admin)
                // ---------------------------------------------------------

                // Rolleri Ekle
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!await roleManager.RoleExistsAsync("Uye"))
                    await roleManager.CreateAsync(new IdentityRole("Uye"));

                // Admin Kullanıcısını Ekle
                var adminEmail = "ogrencinumarasi@sakarya.edu.tr";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        AdSoyad = "Sistem Yöneticisi",
                        EmailConfirmed = true
                    };

                    // Şifre: sau (Program.cs'deki ayarlara göre 3 karakter serbest)
                    var result = await userManager.CreateAsync(newAdmin, "sau");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }

                // ---------------------------------------------------------
                // B. UYGULAMA VERİLERİ (Hizmet ve Antrenör)
                // ---------------------------------------------------------

                // Hizmet yoksa ekle
                if (!context.Hizmetler.Any())
                {
                    context.Hizmetler.AddRange(
                        new Hizmet { Ad = "Fitness", Sure = 60, Ucret = 250 },
                        new Hizmet { Ad = "Yoga", Sure = 45, Ucret = 200 },
                        new Hizmet { Ad = "Pilates", Sure = 45, Ucret = 200 }
                    );
                    await context.SaveChangesAsync();
                }

                // Antrenör yoksa ekle
                if (!context.Antrenorler.Any())
                {
                    context.Antrenorler.AddRange(
                        new Antrenor { AdSoyad = "Ahmet Yılmaz", Uzmanlik = "Fitness" },
                        new Antrenor { AdSoyad = "Ayşe Kaya", Uzmanlik = "Yoga" },
                        new Antrenor { AdSoyad = "Mehmet Demir", Uzmanlik = "Pilates" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}